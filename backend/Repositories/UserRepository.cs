using System.Security.Claims;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public enum ErrorCodes
{
    NotFound,
    CannotRetrieveUserFromCookie,
    UpdateUserDbFailed,
    Forbidden,
    Ok,
}

public class UserRepository : IUserInterface
{
    private readonly UserManager<User> _userManager;
    private readonly IFormsInterface _formService;
    // private readonly FormFiller _formFiller;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserRepository(
        UserManager<User> userManager, 
        IFormsInterface formService,
        // FormFiller formFiller,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _formService = formService;
        _contextAccessor = contextAccessor;
        // _formFiller = formFiller;
    }
    
    public async Task<Tuple<ErrorCodes, ProfileDisplayDto?>> DisplayUserProfile(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.NotFound, null);
        
        var display = CreateDisplayFromPreferences(user);
        return new Tuple<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.Ok, display);
    }
    public async Task<ErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences)
    {
        var userEmail = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        if (userEmail == null)
            return ErrorCodes.CannotRetrieveUserFromCookie;
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        if (user == null)
            return ErrorCodes.NotFound;
        user.Preferences = newUserPreferences;
        
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return ErrorCodes.Ok;
        return ErrorCodes.UpdateUserDbFailed;
    }

    public async Task<Tuple<ErrorCodes, Form[]?>> GetUserForms(string email) {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<ErrorCodes, Form[]?>(ErrorCodes.NotFound, null);
        
        return new Tuple<ErrorCodes, Form[]?>(ErrorCodes.Ok, await _formService.GetAll());
    }

    public async Task<ErrorCodes> ChangeUserProfile(UpdateUserDto userDto)
    {
        var userEmail = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        if (userEmail == null)
            return ErrorCodes.CannotRetrieveUserFromCookie;
        if (userDto.Email != userEmail)
            return ErrorCodes.Forbidden;
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        if (user == null)
            return ErrorCodes.NotFound;
        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.PhoneNumber = userDto.PhoneNumber;
        
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            return ErrorCodes.Ok;
        return ErrorCodes.UpdateUserDbFailed;
    }

    // public async Task<ErrorCodes> SubmitAnswers(AnswerDto answer) 
    // {
    //     // _formFiller.FillForm(answer);
    //     // return ErrorCodes.Ok;
    // }

    // For now, I do not have better idea
    ProfileDisplayDto CreateDisplayFromPreferences(User user)
    {
        var currentUserEmail = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        var isForCurrentUser = currentUserEmail == user.Email;

        var preferences = user.Preferences;
        ProfileDisplayDto dto = new ProfileDisplayDto();
        dto.Email = preferences.DisplayEmail || isForCurrentUser ? user.Email : null;
        dto.FirstName = preferences.DisplayFirstName || isForCurrentUser ? user.FirstName : null;
        dto.LastName = preferences.DisplayLastName || isForCurrentUser ? user.LastName : null;
        dto.PhoneNumber = preferences.DisplayPhoneNumber || isForCurrentUser ? user.PhoneNumber : null;
        return dto;
    }
}