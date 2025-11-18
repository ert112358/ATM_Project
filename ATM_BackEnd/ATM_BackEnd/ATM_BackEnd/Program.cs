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
    if (db.Users.Any(u => u.Name.Equals(username)))
        return Results.BadRequest("Username already taken");
    
    User user = new User
    {
        Name = username,
        Digest = hasher.HashPassword(username, password),
        Token = System.Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
        Balance = 0,
        Transactions = []
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
    
    string hashedPass = hasher.HashPassword(username,  password);
    User? user = db.Users.Find(username);
    
    if (user != null)
    {
        var result = hasher.VerifyHashedPassword(username, user.Digest, password);

        if (result == PasswordVerificationResult.Success)
            return Results.Ok(user.Token);
    }
    return Results.Ok("Bad username or password");

});

app.MapGet("/api/viewbalance", (HttpContext context, ATMContext db) =>
{
    string token = context.Request.Query["token"].ToString().Replace(" ","+");
    
    if (token.IsNullOrEmpty())
        return Results.BadRequest("Bad request");

    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        context.Response.WriteAsJsonAsync(new { user.Name, user.Balance, user.Transactions });
        return Results.Ok();
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
    
    if (token.IsNullOrEmpty() ||  amountString.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    
    int amount = Int32.Parse(amountString);
    
    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        
        if(user.Balance < amount)
            return Results.BadRequest("Insufficient balance");
        
        user.Balance -= amount;
        
        db.Users.Update(user);
        db.SaveChanges();
        
        return Results.Ok(user.Balance);
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest("Bad request");
    }
});

app.MapGet("/api/deposit", (HttpContext context, ATMContext db) =>
{
    string token = context.Request.Query["token"].ToString().Replace(" ","+");
    string amountString = context.Request.Query["amount"].ToString();
    
    if (token.IsNullOrEmpty() ||  amountString.IsNullOrEmpty())
        return Results.BadRequest("Bad request");
    
    int amount = Int32.Parse(amountString);
    
    try
    {
        User user = db.Users.Single(u => u.Token.Equals(token));
        
        user.Balance += amount;
        
        db.Users.Update(user);
        db.SaveChanges();
        
        return Results.Ok(user.Balance);
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest("Bad request");
    }
});

app.Run();