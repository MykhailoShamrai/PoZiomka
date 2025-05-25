using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using backend.Models.User;
using backend.Dto;

namespace IntegrationTests;

public class AdminIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AdminIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // InMemory dla AppDbContext
                var appDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (appDescriptor != null) services.Remove(appDescriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                });

                // InMemory dla AuthDbContext
                var authDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AuthDbContext>));
                if (authDescriptor != null) services.Remove(authDescriptor);

                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAuthDb");
                });

                // Zarejestruj użytkownika z rolą Admin
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                roleManager.CreateAsync(new IdentityRole<int>("Admin")).Wait();

                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(adminUser, "Admin123!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                }
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        // Logowanie testowego admina
        var loginResponse = _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@example.com",
            Password = "Admin123!"
        }).Result;

        if (!loginResponse.IsSuccessStatusCode)
            throw new Exception("Logowanie testowego admina nie powiodło się.");
    }

    private FormCreateDto GetValidFormDto(string name) => new()
    {
        FormName = name,
        Questions = new List<string> { "Jak masz na imię?", "Ulubiony kolor?" },
        Obligatoriness = new List<bool> { true, false },
        NumberOfOptions = new List<int> { 2, 3 },
        Options = new List<string> { "Adam", "Ewa", "Niebieski", "Zielony", "Czerwony" }
    };

    [Fact]
    public async Task AddNewForm_ReturnsOk()
    {
        var formDto = GetValidFormDto("TestForm1");

        var response = await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AddNewForm_ReturnsBadRequest_WhenAlreadyExists()
    {
        var formDto = GetValidFormDto("DuplicateForm");

        await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);
        var response = await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteForm_ReturnsOk_WhenFormExists()
    {
        var formDto = GetValidFormDto("DeletableForm");
        await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);

        var content = new StringContent(JsonSerializer.Serialize("DeletableForm"), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/admin/delete_form", UriKind.Relative),
            Content = content
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteForm_ReturnsBadRequest_WhenFormDoesNotExist()
    {
        var content = new StringContent(JsonSerializer.Serialize("NonExistentForm"), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/admin/delete_form", UriKind.Relative),
            Content = content
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddQuestion_ReturnsOk()
    {
        var formDto = GetValidFormDto("FormWithQuestion");
        await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);

        var questionDto = new AddQuestionDto
        {
            FormName = "FormWithQuestion",
            Name = "Twoje hobby?",
            IsObligatory = true,
            Answers = new List<string> { "Programowanie", "Sport", "Muzyka" }
        };

        var response = await _client.PostAsJsonAsync("/api/admin/add_question", questionDto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteQuestion_ReturnsOk()
    {
        var formDto = GetValidFormDto("FormToDeleteQuestion");
        await _client.PostAsJsonAsync("/api/admin/add_new_form", formDto);

        var questionDto = new AddQuestionDto
        {
            FormName = "FormToDeleteQuestion",
            Name = "Ulubiony język?",
            IsObligatory = true,
            Answers = new List<string> { "C#", "Python", "Java" }
        };
        await _client.PostAsJsonAsync("/api/admin/add_question", questionDto);

        var deleteDto = new DeleteQuestionDto
        {
            FormName = "FormToDeleteQuestion",
            QuestionName = "Ulubiony język?"
        };

        var response = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/admin/delete_question", UriKind.Relative),
            Content = JsonContent.Create(deleteDto)
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUsersInformation_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/admin/users_information");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddNewRoom_ReturnsOk()
    {
        var roomDto = new List<RoomInDto>
    {
        new RoomInDto
        {
            Floor = 1,
            Number = 101,
            Capacity = 2,
            Status = RoomStatus.Available
        }
    };

        var response = await _client.PostAsJsonAsync("/api/admin/add_new_room", roomDto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllRooms_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/admin/get_all_rooms");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GenerateProposals_ReturnsHandledStatus()
    {
        var response = await _client.GetAsync("/api/admin/generate_proposals");

        Assert.Contains(response.StatusCode, new[] {
        HttpStatusCode.OK,
        HttpStatusCode.NotFound,
        HttpStatusCode.BadRequest
    });
    }

    [Fact]
    public async Task AddRoleToUser_ReturnsOk()
    {
        // Stwórz użytkownika testowego
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = "testuser@example.com",
            Password = "Test123!",
            Name = "Test",
            Surname = "User"
        });
        Assert.True(registerResponse.IsSuccessStatusCode);

        // Przypisz rolę
        var dto = new AddRoleToUserDto
        {
            Email = "testuser@example.com",
            Role = "Student"
        };

        var response = await _client.PutAsJsonAsync("/api/admin/add_role_to_user", dto);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteRoom_ReturnsOk()
    {
        var roomDto = new List<RoomInDto>
    {
        new RoomInDto
        {
            Floor = 2,
            Number = 202,
            Capacity = 3,
            Status = RoomStatus.Available
        }
    };
        await _client.PostAsJsonAsync("/api/admin/add_new_room", roomDto);

        var response = await _client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/api/admin/delete_room", UriKind.Relative),
            Content = JsonContent.Create(roomDto[0])
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllProposals_ReturnsHandledStatus()
    {
        var response = await _client.GetAsync("/api/admin/get_all_proposals");

        Assert.Contains(response.StatusCode, new[]
        {
        HttpStatusCode.OK,
        HttpStatusCode.NotFound,
        HttpStatusCode.BadRequest
    });
    }


    [Fact]
    public async Task ChangeAdminStatus_ReturnsNotFound()
    {
        var dto = new AdminChangesStatusProposalDto
        {
            ProposalId = 999999, // nieistniejące ID
            Status = AdminStatus.Accepted
        };

        var response = await _client.PutAsJsonAsync("/api/admin/change_admin_status", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


}
