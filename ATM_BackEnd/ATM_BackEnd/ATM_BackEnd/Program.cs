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

app.Run();