using backend.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// It's important to not forgot to add ConnextionStrings__Devconnection environment variable
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Devconnection")));
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddRoles<IdentityRole<int>>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.InitializeAuthContext();
app.UseRouting();
app.UseCors("AllowAngularApp");
app.MapControllers();

app.Run();