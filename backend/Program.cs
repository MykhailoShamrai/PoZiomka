using backend.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// It's important to not forgot to add ConnextionStrings__Devconnection environment variable
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Devconnection")));
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddRoles<IdentityRole<int>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.InitializeAuthContext();

app.UseHttpsRedirection();
app.Run();