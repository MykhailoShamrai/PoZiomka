using backend.Dto;
using backend.Models.User;

namespace backend.Mappers;

public static class UserDtoMapper
{
    public static UserDto UserToDto(this User user)
    {
        return new UserDto
        {
            Email = user.Email,
            Name = user.FirstName,
            Surname = user.LastName
        };
    }
}