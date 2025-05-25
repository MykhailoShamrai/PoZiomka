using System.Threading.Tasks;
using backend.Data;
using backend.Dto;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class AdminRepositoryIntegrationTests
{
    private readonly AdminRepository _repository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public AdminRepositoryIntegrationTests()
    {
        var services = new ServiceCollection();

        // 🔧 Logowanie wymagane dla UserManager/RoleManager
        services.AddLogging();

        // 💾 InMemory baza
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseInMemoryDatabase("AdminRepoTestDb");
        });

        // 🔐 Identity
        services.AddIdentity<User, IdentityRole<int>>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        var provider = services.BuildServiceProvider();

        _userManager = provider.GetRequiredService<UserManager<User>>();
        _roleManager = provider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        _repository = new AdminRepository(_userManager);

        // 🛠 Upewnij się, że rola Student istnieje
        if (!_roleManager.RoleExistsAsync("Student").Result)
        {
            _roleManager.CreateAsync(new IdentityRole<int>("Student")).Wait();
        }
    }

    [Fact]
    public async Task SetRoleToUser_ReturnsOk_WhenUserExists()
    {
        var user = new User
        {
            UserName = "user1",
            Email = "user1@example.com",
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "Test123!");

        var result = await _repository.SetRoleToUser(new AddRoleToUserDto
        {
            Email = "user1@example.com",
            Role = "Student"
        });

        Assert.Equal(ErrorCodes.Ok, result);
    }

    [Fact]
    public async Task SetRoleToUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var result = await _repository.SetRoleToUser(new AddRoleToUserDto
        {
            Email = "notfound@example.com",
            Role = "Student"
        });

        Assert.Equal(ErrorCodes.NotFound, result);
    }

    [Fact]
    public async Task GetInformationAboutUsers_ReturnsAllUsers()
    {
        await _userManager.CreateAsync(new User { UserName = "a", Email = "a@a.com", EmailConfirmed = true }, "Test123!");
        await _userManager.CreateAsync(new User { UserName = "b", Email = "b@b.com", EmailConfirmed = true }, "Test123!");

        var (list, status) = await _repository.GetInformationAboutUsers();

        Assert.Equal(ErrorCodes.Ok, status);
        Assert.NotNull(list);
        Assert.True(list.Count >= 2);
    }
}
