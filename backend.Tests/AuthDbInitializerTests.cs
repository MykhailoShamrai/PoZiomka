using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AuthDbInitializerTests
{
    [Fact]
    public async Task Initialize_CreatesRoles_WhenTheyDoNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AuthDbContext(options);

        var store = new Mock<IRoleStore<IdentityRole<int>>>().Object;

        var roleManager = new Mock<RoleManager<IdentityRole<int>>>(
            store,
            new IRoleValidator<IdentityRole<int>>[0],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!
        );

        roleManager.Setup(m => m.RoleExistsAsync("Student")).ReturnsAsync(false);
        roleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(false);
        roleManager.Setup(m => m.CreateAsync(It.IsAny<IdentityRole<int>>()))
                   .ReturnsAsync(IdentityResult.Success);

        // Act
        await AuthDbInitializer.Initialize(context, roleManager.Object);

        // Assert
        roleManager.Verify(m => m.CreateAsync(It.Is<IdentityRole<int>>(r => r.Name == "Student")), Times.Once);
        roleManager.Verify(m => m.CreateAsync(It.Is<IdentityRole<int>>(r => r.Name == "Admin")), Times.Once);
    }
}
