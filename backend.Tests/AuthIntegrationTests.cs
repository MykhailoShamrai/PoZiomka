using System.Net;
using System.Net.Http.Json;
using backend.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AuthDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_1"));
                });
            });

            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task Register_Then_Login_ShouldReturnOk()
        {
            var registerDto = new RegisterUserDto
            {
                Email = "admin@example.com",
                Password = "Haslo123!"
            };

            var regRes = await _client.PostAsJsonAsync("/api/Auth/register", registerDto);
            Assert.Equal(HttpStatusCode.OK, regRes.StatusCode);

            var loginDto = new LoginUserDto
            {
                Email = "admin@example.com",
                Password = "Haslo123!"
            };

            var loginRes = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);
            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            var dto = new RegisterUserDto
            {
                Email = "duplicate@example.com",
                Password = "Haslo123!"
            };

            // Pierwsza rejestracja OK
            var first = await _client.PostAsJsonAsync("/api/Auth/register", dto);
            Assert.Equal(HttpStatusCode.OK, first.StatusCode);

            // Druga rejestracja = błąd
            var second = await _client.PostAsJsonAsync("/api/Auth/register", dto);
            Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenPasswordIsWrong()
        {
            var dto = new RegisterUserDto
            {
                Email = "wrongpass@example.com",
                Password = "Correct123!"
            };

            await _client.PostAsJsonAsync("/api/Auth/register", dto);

            var loginDto = new LoginUserDto
            {
                Email = "wrongpass@example.com",
                Password = "WrongPassword!"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Logout_ReturnsOk_AfterLogin()
        {
            var dto = new RegisterUserDto
            {
                Email = "logouttest@example.com",
                Password = "Haslo123!"
            };

            await _client.PostAsJsonAsync("/api/Auth/register", dto);
            await _client.PostAsJsonAsync("/api/Auth/login", new LoginUserDto
            {
                Email = "logouttest@example.com",
                Password = "Haslo123!"
            });

            var response = await _client.PostAsync("/api/Auth/logout", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
