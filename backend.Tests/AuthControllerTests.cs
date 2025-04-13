using Moq;
using backend.Controllers;
using backend.Interfaces;
using backend.Dto;
using Microsoft.AspNetCore.Mvc;

namespace backend.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            // Arrange
            var mockAuthInterface = new Mock<IAuthInterface>();
            var registerDto = new RegisterUserDto
            {
                Email = "test@example.com",
                Password = "Haslo123"
            };

            mockAuthInterface
                .Setup(auth => auth.Register(registerDto))
                .ReturnsAsync(true); // symulujemy sukces

            var controller = new AuthController(mockAuthInterface.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var mockAuthInterface = new Mock<IAuthInterface>();
            var registerDto = new RegisterUserDto
            {
                Email = "existing@example.com",
                Password = "Haslo123"
            };

            mockAuthInterface
                .Setup(auth => auth.Register(registerDto))
                .ReturnsAsync(false); // symulujemy porażkę

            var controller = new AuthController(mockAuthInterface.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
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

            // Act
            var result = await controller.Login(loginDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            // Arrange
            var mockAuthInterface = new Mock<IAuthInterface>();
            var loginDto = new LoginUserDto
            {
                Email = "wrong@example.com",
                Password = "wrongpass"
            };

            mockAuthInterface
                .Setup(auth => auth.Login(loginDto))
                .ReturnsAsync(false); // symulujemy błędne dane

            var controller = new AuthController(mockAuthInterface.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Logout_ReturnsOk()
        {
            // Arrange
            var mockAuthInterface = new Mock<IAuthInterface>();

            mockAuthInterface
                .Setup(auth => auth.Logout())
                .Returns(Task.CompletedTask); // logout nie zwraca nic, tylko async

            var controller = new AuthController(mockAuthInterface.Object);

            // Act
            var result = await controller.Logout();

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}
