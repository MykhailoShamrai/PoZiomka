using backend.Dto;
using backend.Mappers;
using backend.Models.User; // User
using Xunit;

public class UserDtoMapperTests
{
    [Fact]
    public void UserToDto_MapsCorrectProperties()
    {
        var user = new User
        {
            Id = 99,
            Email = "a@b.com",
            FirstName = "Anna",
            LastName = "Nowak"
        };

        var dto = user.UserToDto();

        Assert.Equal(99, dto.Id);
        Assert.Equal("a@b.com", dto.Email);
        Assert.Equal("Anna", dto.Name);
        Assert.Equal("Nowak", dto.Surname);
    }
}
