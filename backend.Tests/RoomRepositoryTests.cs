using backend.Data;
using backend.Dto;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace backend.Tests
{
    public class RoomRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private UserManager<User> GetMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new UserManager<User>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            return mgr;
        }

        [Fact]
        public async Task AddRoom_AddsRoomsSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            var rooms = new List<RoomInDto>
            {
                new RoomInDto { Floor = 1, Number = 101, Capacity = 2, Status = 0 },
                new RoomInDto { Floor = 1, Number = 102, Capacity = 3, Status = 0 }
            };

            var result = await repo.AddRoom(rooms);

            Assert.Equal(ErrorCodes.Ok, result);
            Assert.Equal(2, dbContext.Rooms.Count());
        }

        [Fact]
        public async Task DeleteRoom_ReturnsOk_WhenRoomExists()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            var room = new Room { Floor = 1, Number = 101, Capacity = 2, Status = 0 };
            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            var dto = new RoomInDto { Floor = 1, Number = 101, Capacity = 2, Status = 0 };

            var result = await repo.DeleteRoom(dto);

            Assert.Equal(ErrorCodes.Ok, result);
            Assert.False(await dbContext.Rooms.AnyAsync());
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            var dto = new RoomInDto { Floor = 1, Number = 999, Capacity = 1, Status = 0 };

            var result = await repo.DeleteRoom(dto);

            Assert.Equal(ErrorCodes.NotFound, result);
        }

        [Fact]
        public async Task ChangeStatusForRoom_ChangesStatusSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            var room = new Room { Floor = 1, Number = 101, Capacity = 2, Status = 0 };
            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            var dto = new SetStatusToRoomDto { RoomId = room.Id, Status = RoomStatus.Unavailable };

            var result = await repo.ChangeStatusForRoom(dto);

            Assert.Equal(ErrorCodes.Ok, result);

            var updatedRoom = await dbContext.Rooms.FindAsync(room.Id);

            Assert.Equal(ErrorCodes.Ok, result);
            Assert.Equal(RoomStatus.Unavailable, updatedRoom?.Status);
        }

        [Fact]
        public async Task ChangeStatusForRoom_ReturnsNotFound_WhenRoomNotExist()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            var dto = new SetStatusToRoomDto { RoomId = 999, Status = RoomStatus.Unavailable };

            var result = await repo.ChangeStatusForRoom(dto);

            Assert.Equal(ErrorCodes.NotFound, result);
        }

        [Fact]
        public async Task GetRooms_ReturnsRoomsSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var repo = new RoomRepository(dbContext, GetMockUserManager());

            dbContext.Rooms.Add(new Room { Floor = 1, Number = 101, Capacity = 2, Status = 0 });
            dbContext.Rooms.Add(new Room { Floor = 2, Number = 201, Capacity = 1, Status = 0 });
            await dbContext.SaveChangesAsync();

            var (rooms, errorCode) = await repo.GetRooms();

            Assert.Equal(ErrorCodes.Ok, errorCode);
            Assert.Equal(2, rooms.Count);
        }

        [Fact]
        public async Task ApplyUserToRoom_AddsUserSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var userManager = MockUserManager();
            var repo = new RoomRepository(dbContext, userManager.Object);

            var user = new User { Id = 0, Email = "user@example.com" };
            var room = new Room { Floor = 1, Number = 101, Capacity = 2, Status = RoomStatus.Available, ResidentsIds = new List<int>() };

            userManager.Setup(m => m.FindByEmailAsync("user@example.com"))
                .ReturnsAsync(user);

            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            var dto = new UserRoomDto { RoomId = room.Id, UserEmail = "user@example.com" };
            var result = await repo.ApplyUserToRoom(dto);

            Assert.Equal(ErrorCodes.Ok, result);
            Assert.Contains(user.Id, room.ResidentsIds);
        }

        [Fact]
        public async Task RemoveUserFromRoom_RemovesUserSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var userManager = MockUserManager();
            var repo = new RoomRepository(dbContext, userManager.Object);

            var user = new User { Id = 0, Email = "user@example.com" };
            var room = new Room
            {
                Floor = 1,
                Number = 101,
                Capacity = 2,
                Status = RoomStatus.Available,
                ResidentsIds = new List<int> { 0 }
            };

            userManager.Setup(m => m.FindByEmailAsync("user@example.com"))
                .ReturnsAsync(user);

            dbContext.Rooms.Add(room);
            await dbContext.SaveChangesAsync();

            var dto = new UserRoomDto { RoomId = room.Id, UserEmail = "user@example.com" };
            var result = await repo.RemoveUserFromRoom(dto);

            Assert.Equal(ErrorCodes.Ok, result);
            Assert.DoesNotContain(user.Id, room.ResidentsIds);
            Assert.Equal(RoomStatus.Available, room.Status);
        }


        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns<string>(email => Task.FromResult(new User { Email = email, Id = 0 })!);

            return mgr;
        }
    }
}
