using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Diagnostics;
using backend.Dto;
using backend.Interfaces;
using backend.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace backend.Repositories;

public class AuthRepository : IAuthInterface
{
    private readonly string PathToFileWithPasswords = Path.Combine(AppContext.BaseDirectory, "AdditionalFiles", "xato-net-10-million-passwords-10000.txt");
    private readonly HashSet<string> _weakPasswords = new HashSet<string>();
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthRepository(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        var collection = File.ReadLines(PathToFileWithPasswords);
        foreach (string s in collection)
        {
            _weakPasswords.Add(s);
        }
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

    public async Task<bool> Login(LoginUserDto dto)
    {
        var account = await _userManager.FindByEmailAsync(dto.Email);
    
        if (account is not null)
        {
            if (await _userManager.CheckPasswordAsync(account, dto.Password))
            {
                var claims = await _userManager.GetClaimsAsync(account);
                var roles = await _userManager.GetRolesAsync(account);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var identity = new ClaimsIdentity(claims, Settings.AuthCookieName);
                var principal = new ClaimsPrincipal(identity); 

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(5) 
                };

                var context = _contextAccessor.HttpContext;
                if (context is null)
                    throw new NullReferenceException("HttpContext is null!");
                await context!.SignInAsync(Settings.AuthCookieName, principal, authProperties);
                return true;
            }
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
        // Check if password is in weak passwords collection. 
        if (_weakPasswords.Contains(dto.Password))
            return false;
        var result = await _userManager.CreateAsync(nUser, dto.Password);
        return result.Succeeded;
    }
}