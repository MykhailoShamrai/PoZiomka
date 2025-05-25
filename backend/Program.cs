using backend.Data;
using backend.Interfaces;
using backend.Models.User;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition("Cookie", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Name = Settings.AuthCookieName,
            In = Microsoft.OpenApi.Models.ParameterLocation.Cookie,
            Description = "Use the Cookie for authentication"
        });

        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Cookie"
                    }
                },
                new string[] { }
            }
        });
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Please set "dotnet user-secrets init"
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("AzureConnection");

// Db contexts
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddRoles<IdentityRole<int>>();

// Here we register our interfaces and repositories
builder.Services.AddScoped<IAuthInterface, AuthRepository>();
builder.Services.AddScoped<IFormsInterface, FormService>();
builder.Services.AddScoped<FormFiller>();
builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddScoped<IAdminInterface, AdminRepository>();
builder.Services.AddScoped<IRoomInterface, RoomRepository>();
builder.Services.AddScoped<IProposalInterface, ProposalRepository>();
builder.Services.AddScoped<IJudgeInterface, JudgeService>();
builder.Services.AddScoped<CommunicationSender, CommunicationSender>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = Settings.AuthCookieName;
        options.DefaultChallengeScheme = Settings.AuthCookieName;
        options.DefaultScheme = Settings.AuthCookieName;
    })
    .AddCookie(Settings.AuthCookieName,
    options =>
    {
        options.LoginPath = "/api/Auth/login";
        options.Cookie.Name = Settings.AuthCookieName;
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
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
              .AllowAnyMethod()
              .AllowCredentials();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }