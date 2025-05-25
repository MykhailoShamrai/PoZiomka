using backend.Dto;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class AuthRepositoryIntegrationTests
{
    private readonly AuthRepository _repository;
    private readonly UserManager<User> _userManager;

    public AuthRepositoryIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddHttpContextAccessor();

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseInMemoryDatabase("AuthRepoTestDb");
        });

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

        var provider = services.BuildServiceProvider();

        var context = new DefaultHttpContext
        {
            RequestServices = provider
        };
        services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

        var path = Path.Combine(AppContext.BaseDirectory, "AdditionalFiles");
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "xato-net-10-million-passwords-10000.txt"), "");

        _userManager = provider.GetRequiredService<UserManager<User>>();
        var contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
        _repository = new AuthRepository(_userManager, contextAccessor);

    }

    [Fact]
    public async Task Register_ReturnsTrue_WhenNewUser()
    {
        var result = await _repository.Register(new RegisterUserDto
        {
            Email = "newuser@example.com",
            Password = "StrongPass123!"
        });

        Assert.True(result);
    }

    [Fact]
    public async Task Register_ReturnsFalse_WhenUserExists()
    {
        var dto = new RegisterUserDto
        {
            Email = "existing@example.com",
            Password = "StrongPass123!"
        };

        await _repository.Register(dto);
        var result = await _repository.Register(dto);

        Assert.False(result);
    }

    [Fact]
    public async Task Login_ReturnsFalse_WhenWrongPassword()
    {
        var email = "wrongpass@example.com";
        var password = "StrongPass123!";

        await _repository.Register(new RegisterUserDto
        {
            Email = email,
            Password = password
        });

        var result = await _repository.Login(new LoginUserDto
        {
            Email = email,
            Password = "WrongPassword!"
        });

        Assert.False(result);
    }

    [Fact]
    public async Task Login_ReturnsFalse_WhenUserDoesNotExist()
    {
        var result = await _repository.Login(new LoginUserDto
        {
            Email = "ghost@example.com",
            Password = "NoOne123!"
        });

        Assert.False(result);
    }
}
