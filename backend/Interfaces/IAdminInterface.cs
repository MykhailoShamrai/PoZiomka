using System.Security.Claims;
using backend.Dto;
using backend.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;
namespace backend.Interfaces;

public interface IAdminInterface
{
    public Task<Tuple<List<UserDto>, ErrorCodes>> GetInformationAboutUsers();
    public Task<ErrorCodes> SetRoleToUser(AddRoleToUserDto dto);
}