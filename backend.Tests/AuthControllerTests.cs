using Moq;
using backend.Controllers;
using backend.Interfaces;
using backend.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace backend.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            var mockAuthInterface = new Mock<IAuthInterface>();
            var registerDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Password = "Haslo123"
            };

            mockAuthInterface
                .Setup(auth => auth.Register(registerDto))
                .ReturnsAsync(true);

            var controller = new AuthController(mockAuthInterface.Object);

            var result = await controller.Register(registerDto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            var mockAuthInterface = new Mock<IAuthInterface>();
            var registerDto = new RegisterUserDto
            {
                Email = "existing@example.com",
                Password = "Haslo123"
            };

            mockAuthInterface
                .Setup(auth => auth.Register(registerDto))
                .ReturnsAsync(false);

            var controller = new AuthController(mockAuthInterface.Object);

            var result = await controller.Register(registerDto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginSucceeds()
        {
            var mockAuthInterface = new Mock<IAuthInterface>();
            var loginDto = new LoginUserDto
            {
                Email = "test@example.com",
                Password = "Haslo123"
            };

            mockAuthInterface
                .Setup(auth => auth.Login(loginDto))
                .ReturnsAsync(true);

            var controller = new AuthController(mockAuthInterface.Object);

            var result = await controller.Login(loginDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            var mockAuthInterface = new Mock<IAuthInterface>();
            var loginDto = new LoginUserDto
            {
                Email = "wrong@example.com",
                Password = "wrongpass"
            };

            mockAuthInterface
                .Setup(auth => auth.Login(loginDto))
                .ReturnsAsync(false);

            var controller = new AuthController(mockAuthInterface.Object);

            var result = await controller.Login(loginDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            var mockAuthInterface = new Mock<IAuthInterface>();

            mockAuthInterface
                .Setup(auth => auth.Logout())
                .Returns(Task.CompletedTask);

            var controller = new AuthController(mockAuthInterface.Object);

            var result = await controller.Logout();

            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public void Test_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            var mockAuth = new Mock<IAuthInterface>();
            var controller = new AuthController(mockAuth.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = controller.Test();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authenticated!", unauthorized.Value);
        }

        [Fact]
        public void Test_ReturnsOk_WhenAuthenticated()
        {

            var mockAuth = new Mock<IAuthInterface>();
            var controller = new AuthController(mockAuth.Object);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "admin@example.com")
            }, authenticationType: "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };

            var result = controller.Test();

            Assert.IsType<OkResult>(result);
        }
    }
}