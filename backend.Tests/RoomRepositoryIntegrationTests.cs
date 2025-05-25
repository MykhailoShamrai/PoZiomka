using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Data;
using backend.Dto;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class RoomRepositoryIntegrationTests
    {
        private readonly RoomRepository _repository;
        private readonly AppDbContext _appDb;
        private readonly UserManager<User> _userManager;

        public RoomRepositoryIntegrationTests()
        {
            var services = new ServiceCollection();

            // 1) wstrzykujemy oba DbContexty: AuthDbContext dla Identity i AppDbContext dla danych aplikacji
            services.AddDbContext<AuthDbContext>(opt =>
                opt.UseInMemoryDatabase("AuthTestDb"));
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase("AppTestDb"));

            services.AddHttpContextAccessor();
            services.AddLogging();

            // 2) konfigurujemy Identity tak, by trzymała użytkowników w AuthDbContext
            services.AddIdentity<User, IdentityRole<int>>(opts =>
            {
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AuthDbContext>()    // <<< tu ważne!
            .AddDefaultTokenProviders();

            var provider = services.BuildServiceProvider();

            // 3) ustawiamy HttpContext z zalogowanym userem
            var httpCtx = new DefaultHttpContext
            {
                RequestServices = provider,
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, "student@example.com")
                }, "test"))
            };
            provider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpCtx;

            // 4) pobieramy serwisy
            _appDb = provider.GetRequiredService<AppDbContext>();
            _userManager = provider.GetRequiredService<UserManager<User>>();
            _repository = new RoomRepository(_appDb, _userManager);

            // 5) seedujemy użytkownika (w AuthDbContext!)
            SeedUsersAsync(provider).GetAwaiter().GetResult();
        }

        private static async Task SeedUsersAsync(ServiceProvider provider)
        {
            var userMgr = provider.GetRequiredService<UserManager<User>>();
            // upewniamy się, że baza jest czysta
            await provider.GetRequiredService<AuthDbContext>().Database.EnsureDeletedAsync();
            await provider.GetRequiredService<AuthDbContext>().Database.EnsureCreatedAsync();

            var user = new User
            {
                UserName = "student",
                Email = "student@example.com",
                EmailConfirmed = true
            };
            await userMgr.CreateAsync(user, "Test123!");
        }

        [Fact]
        public async Task AddRoom_ReturnsOk()
        {
            var dto = new RoomInDto
            {
                Floor = 1,
                Number = 100,
                Capacity = 2,
                Status = RoomStatus.Available
            };
            var result = await _repository.AddRoom(new List<RoomInDto> { dto });
            Assert.Equal(ErrorCodes.Ok, result);
        }

        [Fact]
        public async Task GetRooms_ReturnsList()
        {
            var (rooms, code) = await _repository.GetRooms();
            Assert.Equal(ErrorCodes.Ok, code);
            Assert.NotNull(rooms);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsOk_WhenExists()
        {
            var dto = new RoomInDto { Floor = 2, Number = 200, Capacity = 1, Status = RoomStatus.Available };
            await _repository.AddRoom(new List<RoomInDto> { dto });

            var result = await _repository.DeleteRoom(dto);
            Assert.Equal(ErrorCodes.Ok, result);
        }

        [Fact]
        public async Task ChangeStatusForRoom_ReturnsOk()
        {
            var dto = new RoomInDto { Floor = 3, Number = 300, Capacity = 1, Status = RoomStatus.Available };
            await _repository.AddRoom(new List<RoomInDto> { dto });

            var room = await _appDb.Rooms.FirstAsync(r => r.Number == 300);
            var result = await _repository.ChangeStatusForRoom(new SetStatusToRoomDto
            {
                RoomId = room.Id,
                Status = RoomStatus.Unavailable
            });
            Assert.Equal(ErrorCodes.Ok, result);
        }

        [Fact]
        public async Task ApplyUserToRoom_ReturnsOk()
        {
            var user = await _userManager.FindByEmailAsync("student@example.com");
            var dto = new RoomInDto { Floor = 4, Number = 400, Capacity = 1, Status = RoomStatus.Available };
            await _repository.AddRoom(new List<RoomInDto> { dto });

            var room = await _appDb.Rooms.FirstAsync(r => r.Number == 400);
            var result = await _repository.ApplyUserToRoom(new UserRoomDto
            {
                RoomId = room.Id,
                UserEmail = user?.Email!
            });
            Assert.Equal(ErrorCodes.Ok, result);
        }

        [Fact]
        public async Task RemoveUserFromRoom_ReturnsOk()
        {
            var user = await _userManager.FindByEmailAsync("student@example.com");
            var dto = new RoomInDto { Floor = 5, Number = 500, Capacity = 1, Status = RoomStatus.Available };
            await _repository.AddRoom(new List<RoomInDto> { dto });

            var room = await _appDb.Rooms.FirstAsync(r => r.Number == 500);
            await _repository.ApplyUserToRoom(new UserRoomDto
            {
                RoomId = room.Id,
                UserEmail = user?.Email!
            });

            var result = await _repository.RemoveUserFromRoom(new UserRoomDto
            {
                RoomId = room.Id,
                UserEmail = user?.Email!
            });
            Assert.Equal(ErrorCodes.Ok, result);
        }
    }
}
