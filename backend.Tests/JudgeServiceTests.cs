using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using backend.Models.User;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace backend.Tests.Services
{
    public class JudgeServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private UserManager<User> GetUserManagerMock(IEnumerable<User> usersInRole)
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mgr.Setup(x => x.GetUsersInRoleAsync("Student"))
               .ReturnsAsync(usersInRole.ToList());

            return mgr.Object;
        }

        [Fact]
        public async Task GenerateProposals_ReturnsOk_WhenSingleActiveStudentAndOneRoom()
        {
            var db = GetInMemoryDbContext();
            db.Rooms.Add(new Room { Capacity = 1, Status = RoomStatus.Available });
            await db.SaveChangesAsync();

            var student = new User { Id = 42, StudentStatus = StudentStatus.Active };
            var userManager = GetUserManagerMock(new[] { student });

            var service = new JudgeService(db, userManager);

            var result = await service.GenerateProposals();

            Assert.Equal(JudgeError.Ok, result);

            var proposals = await db.Set<Proposal>().ToListAsync();
            Assert.Single(proposals);

            Assert.Contains(42, proposals[0].RoommatesIds);
        }

        [Fact]
        public async Task GenerateProposals_ReturnsDatabaseError_WhenSaveChangesFails()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new FailingDbContext(options);

            db.Rooms.Add(new Room { Capacity = 1, Status = RoomStatus.Available });
            await db.SaveChangesAsync();

            var student = new User { Id = 7, StudentStatus = StudentStatus.Active };
            var userManager = GetUserManagerMock([student]);

            var service = new JudgeService(db, userManager);

            var result = await service.GenerateProposals();

            Assert.Equal(JudgeError.DatabaseError, result);
        }

    }

    public class FailingDbContext : AppDbContext
    {
        public FailingDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);
            return 0;
        }
    }

}
