using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace backend.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IFormsInterface> _formServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        _formServiceMock = new Mock<IFormsInterface>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _repository = new UserRepository(
            _userManagerMock.Object,
            _formServiceMock.Object,
            _httpContextAccessorMock.Object
        );
    }

    [Fact]
    public async Task DisplayUserProfile_ReturnsNotFound_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync("email@test.com"))
            .ReturnsAsync((User)null);

        var result = await _repository.DisplayUserProfile("email@test.com");

        Assert.Equal(ErrorCodes.NotFound, result.Item1);
        Assert.Null(result.Item2);
    }

    [Fact]
    public async Task DisplayUserProfile_ReturnsOk_WhenUserFound()
    {
        var user = new User
        {
            Email = "email@test.com",
            Preferences = new UserPreferences()
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, user.Email)
        }));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = await _repository.DisplayUserProfile(user.Email);

        Assert.Equal(ErrorCodes.Ok, result.Item1);
        Assert.NotNull(result.Item2);
    }

    [Fact]
    public async Task ChangeUserPreferences_ReturnsUnauthorized_WhenNoEmail()
    {
        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(new DefaultHttpContext()); // Brak User.Claims

        var result = await _repository.ChangeUserPreferences(new UserPreferences());

        Assert.Equal(ErrorCodes.CannotRetrieveUserFromCookie, result);
    }

    [Fact]
    public async Task GetUserForms_ReturnsNotFound_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByEmailAsync("email@test.com"))
            .ReturnsAsync((User)null);

        var result = await _repository.GetUserForms("email@test.com");

        Assert.Equal(ErrorCodes.NotFound, result.Item1);
        Assert.Null(result.Item2);
    }

    [Fact]
    public async Task GetUserForms_ReturnsForms_WhenUserExists()
    {
        var user = new User { Email = "email@test.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _formServiceMock.Setup(f => f.GetAll()).ReturnsAsync(Array.Empty<Form>());

        var result = await _repository.GetUserForms(user.Email);

        Assert.Equal(ErrorCodes.Ok, result.Item1);
        Assert.NotNull(result.Item2);
    }

    [Fact]
    public async Task ChangeUserProfile_ReturnsForbidden_WhenEmailsDontMatch()
    {
        var userDto = new UpdateUserDto { Email = "email@1.com" };
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "email@2.com")
        }));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        var result = await _repository.ChangeUserProfile(userDto);

        Assert.Equal(ErrorCodes.Forbidden, result);
    }

    [Fact]
    public async Task ChangeUserProfile_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userDto = new UpdateUserDto { Email = "email@test.com" };
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, userDto.Email)
        }));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        _userManagerMock.Setup(m => m.FindByEmailAsync(userDto.Email))
            .ReturnsAsync((User)null);

        var result = await _repository.ChangeUserProfile(userDto);

        Assert.Equal(ErrorCodes.NotFound, result);
    }

    [Fact]
    public async Task ChangeUserProfile_ReturnsOk_WhenUpdateSuccessful()
    {
        var userDto = new UpdateUserDto
        {
            Email = "email@test.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789"
        };

        var user = new User { Email = userDto.Email };

        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, userDto.Email)
        }));
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        _userManagerMock.Setup(m => m.FindByEmailAsync(userDto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _repository.ChangeUserProfile(userDto);

        Assert.Equal(ErrorCodes.Ok, result);
    }
}
