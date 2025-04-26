using backend.Dto;
using backend.Repositories;
namespace backend.Interfaces;

public interface IAdminInterface
{
    public Task<Tuple<List<UserDto>, ErrorCodes>> GetInformationAboutUsers();
}