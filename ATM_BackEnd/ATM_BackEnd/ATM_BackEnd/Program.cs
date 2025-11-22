using System.Buffers.Text;
using System.Security.Cryptography;
using ATM_BackEnd;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<ATMContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ATMContext")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ATMContext>();
    db.Database.Migrate();
}

PasswordHasher<string> hasher = new PasswordHasher<string>();

app.UseHttpsRedirection();

app.MapGet("/api/register", (HttpContext context, ATMContext db) =>
{
    string username = context.Request.Query["username"].ToString();
    string password = context.Request.Query["password"].ToString();
    
    if (username.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    if (password.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    
    if (!Password.IsPasswordStrong(password))
        return Results.BadRequest("The password doesn't meet the safety criteria");
    
    if (db.Users.Any(u => u.Name.Equals(username)))
        return Results.BadRequest("Username already taken");
    
    User user = new User
    {
        Name = username,
        Digest = hasher.HashPassword(username, password),
        Token = System.Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        Balance = 0
    };
    
    db.Users.Add(user);
    db.SaveChanges();
    
    return Results.Ok("OK");
});

app.MapGet("/api/login", (HttpContext context, ATMContext db) =>
{
    string username = context.Request.Query["username"].ToString();
    string password = context.Request.Query["password"].ToString();
    
    if (username.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    if (password.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    
    User? user = db.Users.Find(username);
    
    if (user != null)
    {
        var result = hasher.VerifyHashedPassword(username, user.Digest, password);

        if (result == PasswordVerificationResult.Success)
            return Results.Ok(user.Token);
    }

    return Results.Unauthorized();

});

app.MapGet("/api/viewbalance", (HttpContext context, ATMContext db) =>
{
    string token = context.Request.Query["token"].ToString().Replace(" ","+");
    
    if (token.IsNullOrEmpty())
        return Results.BadRequest("Bad request");

    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        var transactions = db.Transactions.Where(t => t.UserName.Equals(user.Name));
        return Results.Ok(new { user.Name, user.Balance, transactions });
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest("Bad request");
    }

});

app.MapGet("/api/withdraw", (HttpContext context, ATMContext db) =>
{
    string token = context.Request.Query["token"].ToString().Replace(" ","+");
    string amountString = context.Request.Query["amount"].ToString();
    int amount;
    
    if (token.IsNullOrEmpty() ||  amountString.IsNullOrEmpty())
        return Results.BadRequest("Bad request");

    try
    {
        amount = Int32.Parse(amountString);
    }
    catch (FormatException)
    {
        return Results.BadRequest("Bad request");
    }
    catch (OverflowException)
    {
        return Results.BadRequest("The requested value is too high");
    }
    
    if (amount <= 0)
        return Results.BadRequest("Bad request");
    
    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        
        if(user.Balance < amount)
            return Results.BadRequest("Insufficient balance");
        
        user.Balance -= amount;
        db.Transactions.Add(new ATMTransaction{
            Id = Guid.NewGuid(),
            Type = ATMTransactionType.WITHDRAW,
            Amount = amount,
            UserName = user.Name
        });
        
        //db.Users.Update(user);
        db.SaveChanges();
        
        return Results.Ok(user.Balance);
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
});

app.MapGet("/api/deposit", (HttpContext context, ATMContext db) =>
{
    string token = context.Request.Query["token"].ToString().Replace(" ","+");
    string amountString = context.Request.Query["amount"].ToString();
    int amount;
    
    if (token.IsNullOrEmpty() ||  amountString.IsNullOrEmpty())
        return Results.BadRequest("Bad request");

    try
    {
        amount = Int32.Parse(amountString);
    }
    catch (FormatException)
    {
        return Results.BadRequest("Bad request");
    }
    catch (OverflowException)
    {
        return Results.BadRequest("The requested value is too high");
    }
    
    if (amount <= 0)
        return Results.BadRequest("Bad request");
    
    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        
        user.Balance += amount;
        db.Transactions.Add(new ATMTransaction{
            Id = Guid.NewGuid(),
            Type = ATMTransactionType.DEPOSIT,
            Amount = amount,
            UserName = user.Name
        });
        
        //db.Users.Update(user);
        db.SaveChanges();
        
        return Results.Ok(user.Balance);
    }
    catch (InvalidOperationException)
    {
        return Results.Unauthorized();
    }
});

app.MapGet("/api/changepassword", (HttpContext context, ATMContext db) =>
{
    string username = context.Request.Query["username"].ToString();
    string oldpassword = context.Request.Query["oldpassword"].ToString();
    string newpassword = context.Request.Query["newpassword"].ToString();

    if (username.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    if (oldpassword.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    if (newpassword.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    
    if (!Password.IsPasswordStrong(newpassword))
        return Results.BadRequest("The password doesn't meet the safety criteria");
    
    User? user = db.Users.Find(username);
    
    if (user != null)
    {
        var result = hasher.VerifyHashedPassword(username, user.Digest, oldpassword);

        if (result == PasswordVerificationResult.Success)
        {
            if (oldpassword.Equals(newpassword))
                return Results.BadRequest("Old password and new password are the same");
            
            user.Digest = hasher.HashPassword(username, newpassword);
            user.Token = System.Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            
            db.Users.Update(user);
            db.SaveChanges();
            
            return Results.Ok(user.Token);
        }
    }

    return Results.Unauthorized();
});

app.Run();