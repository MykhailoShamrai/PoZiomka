using System.Runtime.CompilerServices;
using System.Security.Claims;
using backend.Dto;
using backend.Interfaces;
using backend.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace backend.Repositories;

public class AuthRepository : IAuthInterface
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthRepository(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    
    // Interface functions

    public async Task<bool> Register(RegisterUserDto dto)
    {
        if (await CheckIfUserExist(dto.Email))
        {
            Console.WriteLine("User already exists!");
            return false;
        }

        if (!await CreateNewUser(dto))
        {
            Console.WriteLine("User creation failed");
            return false;
        }
        return true;
    }

    public async Task<bool> Login(LoginUserDto dto)
    {
        var account = await _userManager.FindByEmailAsync(dto.Email);

        if (account is not null)
        {
            var claims = await _userManager.GetClaimsAsync(account);
            var identity = new ClaimsIdentity(claims, Settings.AuthCookieName);
            var principal = new ClaimsPrincipal(identity); 

            var context = _contextAccessor.HttpContext;
            if (context is null)
                throw new NullReferenceException("HttpContext is null!");
            await context!.SignInAsync(Settings.AuthCookieName, principal);
            return true;
        }

        return false;
    }

    public async Task Logout()
    {
        var context = _contextAccessor.HttpContext;
        if (context is null)
            throw new NullReferenceException("HttpContext is null!");
        await context!.SignOutAsync(Settings.AuthCookieName);
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