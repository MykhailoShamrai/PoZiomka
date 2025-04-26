using backend.Dto;
using backend.Interfaces;
using backend.Mappers;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace backend.Repositories;

public class AdminRepository : IAdminInterface
{
    private readonly UserManager<User> _userManager;
    public AdminRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Tuple<List<UserDto>, ErrorCodes>> GetInformationAboutUsers()
    {
        var users = _userManager.Users;
        if (users is null)
        {
            return new Tuple<List<UserDto>, ErrorCodes>(new List<UserDto>(), ErrorCodes.NotFound);
        }
        return new Tuple<List<UserDto>, ErrorCodes>(await users.Select(u => u.UserToDto()).ToListAsync(), ErrorCodes.Ok);
    }
}