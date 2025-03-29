using System.Diagnostics;
using backend.Dto;
using backend.Interfaces;
using backend.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories;

public class AuthRepository : IAuthInterface
{
    private readonly UserManager<User> _userManager;

    public AuthRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    // Interface functions

    public async Task<bool> Register(RegisterUserDto dto)
    {
        if (await CheckIfUserExist(dto.Email))
        {
            Debug.WriteLine("User already exists!");
            return false;
        }

        if (!await CreateNewUser(dto))
        {
            Debug.WriteLine("User creation failed");
            return false;
        }
        
        return true;
    }

    public async Task<bool> Login()
    {
        throw new NotImplementedException();
    }
    
    // Helper infrastructure
    
    private async Task<bool> CheckIfUserExist(string email)
    {
        var user = await _userManager.FindByEmailAsync(email!);
        return !(user is null);
    }
    
    
    /// <summary>
    /// We now say that username is also an email of a user.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    private async Task<bool> CreateNewUser(RegisterUserDto dto)
    {
        User nUser = new User
        {
            UserName = dto.Email,
            Email = dto.Email
        };
        var result = await _userManager.CreateAsync(nUser, dto.Password);
        return result.Succeeded;
    }
}