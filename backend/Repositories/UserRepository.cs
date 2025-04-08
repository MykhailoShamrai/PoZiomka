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
        
        var preferences = await _appContext.DisplayPreferences.
            FirstOrDefaultAsync(x => x.Id == user.DisplayPreferencesId);
        if (preferences == null)
            return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.PreferencesNotFound, null);
        
        var display = CreateDisplayFromPreferences(preferences);
        return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.Ok, display);
    }
    
    public async Task<UserErrorCodes> ChangeUserPreferences(DisplayPreferences displayPreferences)
    {
        throw new NotImplementedException();
    }

    ProfileDisplayDto CreateDisplayFromPreferences(DisplayPreferences displayPreferences)
    {
        throw new NotImplementedException();
    }
}