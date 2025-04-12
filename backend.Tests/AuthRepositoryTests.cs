using Xunit;
using Moq;
using backend.Repositories;
using backend.Dto;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Tests
{
    public class AuthRepositoryTests
    {
        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        }

        [Fact]
        public async Task Register_CreatesUser_WhenUserDoesNotExist()
        {
            var mockUserManager = MockUserManager();
            var mockHttp = new Mock<IHttpContextAccessor>();

            mockUserManager.Setup(m => m.FindByEmailAsync("test@example.com"))
                           .ReturnsAsync((User?)null);
            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), "Haslo123"))
                           .ReturnsAsync(IdentityResult.Success);

            var repo = new AuthRepository(mockUserManager.Object, mockHttp.Object);
            var result = await repo.Register(new RegisterUserDto { Email = "test@example.com", Password = "Haslo123" });

            Assert.True(result);
        }

        [Fact]
        public async Task Register_ReturnsFalse_WhenUserExists()
        {
            var mockUserManager = MockUserManager();
            mockUserManager.Setup(m => m.FindByEmailAsync("existing@example.com"))
                           .ReturnsAsync(new User());

            var repo = new AuthRepository(mockUserManager.Object, new Mock<IHttpContextAccessor>().Object);
            var result = await repo.Register(new RegisterUserDto { Email = "existing@example.com", Password = "123" });

            Assert.False(result);
        }

        [Fact]
        public async Task Login_ReturnsFalse_WhenUserExists()
        {
            var mockUserManager = MockUserManager();
            var user = new User { Email = "test@example.com" };
            var mockContextAccessor = new Mock<IHttpContextAccessor>();

            var mockAuthService = new Mock<IAuthenticationService>();
            mockUserManager.Setup(m => m.FindByEmailAsync(user.Email))
                           .ReturnsAsync(user);
            mockUserManager.Setup(m => m.GetClaimsAsync(user))
                           .ReturnsAsync(new List<Claim>());
            mockUserManager.Setup(m => m.GetRolesAsync(user))
                           .ReturnsAsync(new List<string>());

            var mockHttpContext = new DefaultHttpContext();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockAuthService.Object)
                .BuildServiceProvider();
            mockHttpContext.RequestServices = serviceProvider;
            mockContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var repo = new AuthRepository(mockUserManager.Object, mockContextAccessor.Object);

            var result = await repo.Login(new LoginUserDto { Email = user.Email, Password = "fake" });

            Assert.False(result);
        }
    
        [Fact]
        public async Task Logout_CallsSignOut()
        {
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var mockUserManager = MockUserManager();
            var mockAuthService = new Mock<IAuthenticationService>();

            var mockHttpContext = new DefaultHttpContext();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(mockAuthService.Object)
                .BuildServiceProvider();
            mockHttpContext.RequestServices = serviceProvider;

            mockContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var repo = new AuthRepository(mockUserManager.Object, mockContextAccessor.Object);
            await repo.Logout();

            mockAuthService.Verify(auth => auth.SignOutAsync(
                It.IsAny<HttpContext>(),
                "PoZiomka_Cookie",
                null), Times.Once);
        }

        [Fact]
        public async Task Logout_ThrowsException_WhenHttpContextIsNull()
        {
            var mockUserManager = MockUserManager();
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            mockContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null!);

            var repo = new AuthRepository(mockUserManager.Object, mockContextAccessor.Object);

            await Assert.ThrowsAsync<NullReferenceException>(() => repo.Logout());
        }

        [Fact]
        public async Task CreateNewUser_ShouldReturnFalse_WhenPasswordIsWeak()
        {
            var mockUserManager = MockUserManager();
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var authRepository = new AuthRepository(mockUserManager.Object, mockContextAccessor.Object);

            var weakPassword = "michael"; 
            var weakPasswordsField = authRepository.GetType()
                .GetField("_weakPasswords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var weakPasswordsSet = weakPasswordsField?.GetValue(authRepository) as HashSet<string>;

            if (weakPasswordsSet == null)
                throw new Exception("Failed to inject weak password");

            weakPasswordsSet.Add(weakPassword);

            var registerDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Password = weakPassword
            };

            var result = await authRepository.Register(registerDto);

            Assert.False(result);
        }
    }
}
