using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Repositories;


namespace backend.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserInterface> _userRepoMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<AppDbContext> _dbContextMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userRepoMock = new Mock<IUserInterface>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _dbContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _controller = new UserController(
                _userRepoMock.Object,
                _userManagerMock.Object,
                _dbContextMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task ChangeMyPreferences_ReturnsOk_WhenSuccessful()
        {
            var prefs = new UserPreferences();
            _userRepoMock.Setup(repo => repo.ChangeUserPreferences(prefs)).ReturnsAsync(ErrorCodes.Ok);

            var result = await _controller.ChangeMyPreferences(prefs);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DisplayProfile_ReturnsUnauthorized_WhenEmailIsMissing()
        {
            var emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _httpContextAccessorMock.Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext { User = emptyPrincipal });

            var result = await _controller.DisplayProfile();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DisplayProfile_ReturnsOk_WhenSuccessful()
        {
            // Arrange: tworzenie tożsamości z e-mailem
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, "test@example.com") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            _httpContextAccessorMock.Setup(c => c.HttpContext).Returns(httpContext);

            // Załóżmy, że zwracany typ to User
            var profileDto = new ProfileDisplayDto
            {
                // Wypełnij wymagane pola
                FirstName = "TestUser",
                Email = "test@example.com"
            };

            _userRepoMock.Setup(repo => repo.DisplayUserProfile("test@example.com"))
                .Returns(Task.FromResult(Tuple.Create(ErrorCodes.Ok, profileDto)));



            // Act
            var result = await _controller.DisplayProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(profileDto, okResult.Value);
        }


        [Fact]
        public async Task ChangeProfile_ReturnsOk_WhenSuccessful()
        {
            var dto = new UpdateUserDto();
            _userRepoMock.Setup(r => r.ChangeUserProfile(dto)).ReturnsAsync(ErrorCodes.Ok);

            var result = await _controller.ChangeProfile(dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetForms_ReturnsUnauthorized_WhenEmailMissing()
        {
            var emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _httpContextAccessorMock.Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext { User = emptyPrincipal });

            var result = await _controller.GetForms();

            Assert.IsType<UnauthorizedResult>(result);
        }

    }
}
