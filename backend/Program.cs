using backend.Interfaces;
using backend.Models.Users;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Please set "dotnet user-secrets init"
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddRoles<IdentityRole<int>>();


builder.Services.AddScoped<IAuthInterface, AuthRepository>();

builder.Services.AddAuthentication()
    .AddCookie(Settings.AuthCookieName,
    options =>
    {
        options.LoginPath = "/api/login";
        options.Cookie.Name = Settings.AuthCookieName;
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        options.AddPolicy("Student", policy => policy.RequireRole("Student"));
    });

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
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseCors("AllowAngularApp");
app.MapControllers();

app.Run();