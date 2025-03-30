using System.Net;
using System.Net.Http.Json;
using backend;
using backend.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using Xunit;

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
                    // Usuwamy oryginalny DbContext
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Dodajemy in-memory DB dla AuthDbContext
                    services.AddDbContext<AuthDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_1"));
                });
            });

            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task Register_Then_Login_ShouldReturnOk()
        {
            // Register
            var registerDto = new RegisterUserDto
            {
                Email = "admin@example.com",
                Password = "Haslo123!"
            };

            var regRes = await _client.PostAsJsonAsync("/api/Auth/register", registerDto);
            Assert.Equal(HttpStatusCode.OK, regRes.StatusCode);

            // Login
            var loginDto = new LoginUserDto
            {
                Email = "admin@example.com",
                Password = "Haslo123!"
            };

            var loginRes = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);
            Assert.Equal(HttpStatusCode.OK, loginRes.StatusCode);
        }
    }
}
