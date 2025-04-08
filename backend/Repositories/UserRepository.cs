using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public enum UserErrorCodes
{
    UserNotFound,
    PreferencesNotFound,
    Ok,
}

public class UserRepository : IUserInterface
{
    private readonly AppDbContext _appContext;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserRepository(AppDbContext appContext, UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _appContext = appContext;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    
    public async Task<Tuple<UserErrorCodes, ProfileDisplayDto?>> DisplayUserProfile(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.UserNotFound, null);
        
        var display = CreateDisplayFromPreferences(user.Preferences);
        return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.Ok, display);
    }
    
    public async Task<UserErrorCodes> ChangeUserPreferences(bool[] displayPreferences)
    {
        throw new NotImplementedException();
    }

    ProfileDisplayDto CreateDisplayFromPreferences(UInt64 displayPreferences)
    {
        throw new NotImplementedException();
    }
}