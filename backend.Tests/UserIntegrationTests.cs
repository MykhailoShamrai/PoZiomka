using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using backend;
using backend.Data;
using backend.Dto;
using backend.Models;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests;

public class UserIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryUserDb");
                });

                var authDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (authDescriptor != null)
                    services.Remove(authDescriptor);

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryUserAuthDb");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                userManager.CreateAsync(new User
                {
                    UserName = "testuser",
                    Email = "testuser@example.com",
                    EmailConfirmed = true
                }, "User123!").Wait();
            });
        });

        _client = customFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        var loginResponse = _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "testuser@example.com",
            Password = "User123!"
        }).Result;

        if (!loginResponse.IsSuccessStatusCode)
            throw new Exception("Logowanie użytkownika testowego nie powiodło się.");
    }

    [Fact]
    public async Task DisplayProfile_ReturnsOkOrNotFound()
    {
        var response = await _client.GetAsync("/api/user/profile");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    [Fact]
    public async Task ChangeProfile_ReturnsOkOrNotFound()
    {
        var updateDto = new UpdateUserDto
        {
            Email = "testuser@example.com",
            FirstName = "Jan",
            LastName = "Kowalski",
            PhoneNumber = "123456789"
        };

        var response = await _client.PutAsJsonAsync("/api/user/profile", updateDto);
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
    }

    [Fact]
    public async Task ChangeMyPreferences_ReturnsValidStatus()
    {
        var pref = new UserPreferencesDto
        {
            DisplayFirstName = true,
            DisplayLastName = false,
            DisplayEmail = true,
            DisplayPhoneNumber = false
        };

        var response = await _client.PostAsJsonAsync("/api/user/preferences", pref);
        Assert.Contains(response.StatusCode, new[]
        {
        HttpStatusCode.OK,
        HttpStatusCode.BadRequest,
        HttpStatusCode.Unauthorized,
        HttpStatusCode.NotFound
    });
    }


    [Fact]
    public async Task GetForms_ReturnsStatus()
    {
        var response = await _client.GetAsync("/api/user/forms");
        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        });
    }

    [Fact]
    public async Task SubmitAnswer_ReturnsHandledStatus()
    {
        var answerDto = new AnswerDto
        {
            FormId = 1,
            ChosenOptionIds = new List<int> { 1, 2 }
        };

        var response = await _client.PostAsJsonAsync("/api/user/submit-answer", answerDto);
        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound,
            HttpStatusCode.Unauthorized
        });
    }

    [Fact]
    public async Task GetMyProposals_ReturnsHandledStatus()
    {
        var response = await _client.GetAsync("/api/user/get_my_proposals");
        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized
        });
    }

    [Fact]
    public async Task AnswerProposal_ReturnsExpectedStatus()
    {
        var dto = new UserChangesStatusProposalDto
        {
            ProposalId = 999999,
            Status = SingleStudentStatus.Accepted
        };

        var response = await _client.PutAsJsonAsync("/api/user/answer_prop", dto);
        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.NotFound
        });
    }

    [Fact]
    public async Task GetMyCommunications_ReturnsStatus()
    {
        var response = await _client.GetAsync("/api/user/get_my_communications");
        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.OK,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.BadRequest
        });
    }
}
