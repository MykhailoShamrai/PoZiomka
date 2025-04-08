using System.Security.Claims;
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
    CannotRetrieveUserFromCookie,
    UpdateUserDbFailed,
    Ok,
}

public class UserRepository : IUserInterface
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserRepository(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    
    public async Task<Tuple<UserErrorCodes, ProfileDisplayDto?>> DisplayUserProfile(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.UserNotFound, null);
        
        var display = CreateDisplayFromPreferences(user);
        return new Tuple<UserErrorCodes, ProfileDisplayDto?>(UserErrorCodes.Ok, display);
    }
    public async Task<UserErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences)
    {
        var userEmail = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        if (userEmail == null)
            return UserErrorCodes.CannotRetrieveUserFromCookie;
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        if (user == null)
            return UserErrorCodes.UserNotFound;
        user.Preferences = newUserPreferences;
        
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return UserErrorCodes.Ok;
        return UserErrorCodes.UpdateUserDbFailed;
    }

    // For now, I do not have better idea
    ProfileDisplayDto CreateDisplayFromPreferences(User user)
    {
        var preferences = user.Preferences;
        ProfileDisplayDto dto = new ProfileDisplayDto();
        dto.Email = preferences.DisplayEmail ? user.Email : null;
        dto.FirstName = preferences.DisplayFirstName ? user.FirstName : null;
        dto.LastName = preferences.DisplayLastName ? user.LastName : null;
        dto.PhoneNumber = preferences.DisplayPhoneNumber ? user.PhoneNumber : null;
        return dto;
    }
}