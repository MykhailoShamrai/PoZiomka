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
    Unauthorized,
    BadRequest
}

public class UserRepository : IUserInterface
{
    private readonly UserManager<User> _userManager;
    private readonly IFormsInterface _formService;
    // private readonly FormFiller _formFiller;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AppDbContext _appDbContext;
    public UserRepository(
        UserManager<User> userManager,
        IFormsInterface formService,
        // FormFiller formFiller,
        IHttpContextAccessor contextAccessor,
        AppDbContext appDbContext)
    {
        _userManager = userManager;
        _formService = formService;
        _contextAccessor = contextAccessor;
        _appDbContext = appDbContext;
        // _formFiller = formFiller;
    }

    public async Task<Tuple<ErrorCodes, ProfileDisplayDto?>> DisplayUserProfile()
    {
        var email = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return new Tuple<ErrorCodes, ProfileDisplayDto?> (ErrorCodes.Unauthorized, null);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.NotFound, null);

        var display = CreateDisplayFromPreferences(user);
        return new Tuple<ErrorCodes, ProfileDisplayDto?>(ErrorCodes.Ok, display);
    }
    public async Task<ErrorCodes> ChangeUserPreferences(UserPreferences newUserPreferences)
    {
        var userEmail = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
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

    public async Task<Tuple<ErrorCodes, Form[]?>> GetUserForms()
    {
        var email = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return new Tuple<ErrorCodes, Form[]?>(ErrorCodes.Unauthorized, null);
            
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new Tuple<ErrorCodes, Form[]?>(ErrorCodes.NotFound, null);

        return new Tuple<ErrorCodes, Form[]?>(ErrorCodes.Ok, await _formService.GetAll());
    }

    public async Task<ErrorCodes> ChangeUserProfile(UpdateUserDto userDto)
    {
        var userEmail = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
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
        var currentUserEmail = _contextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var isForCurrentUser = currentUserEmail == user.Email;

        var preferences = user.Preferences;
        ProfileDisplayDto dto = new ProfileDisplayDto();
        dto.Email = preferences.DisplayEmail || isForCurrentUser ? user.Email : null;
        dto.FirstName = preferences.DisplayFirstName || isForCurrentUser ? user.FirstName : null;
        dto.LastName = preferences.DisplayLastName || isForCurrentUser ? user.LastName : null;
        dto.PhoneNumber = preferences.DisplayPhoneNumber || isForCurrentUser ? user.PhoneNumber : null;
        dto.Preferences = new UserPreferencesDto
        {
            DisplayEmail = preferences.DisplayEmail,
            DisplayFirstName = preferences.DisplayFirstName,
            DisplayLastName = preferences.DisplayLastName,
            DisplayPhoneNumber = preferences.DisplayPhoneNumber
        };
        return dto;
    }

    public async Task<ErrorCodes> SubmitAnswerForForms(AnswerDto dto)
    {
        var email = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email is null)
            return ErrorCodes.Unauthorized;
        
        // Fetch the form
        var form = await _appDbContext.Forms
            .FirstOrDefaultAsync(f => f.FormId == dto.FormId);
        
        if (form is null)
            return ErrorCodes.NotFound;

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return ErrorCodes.Unauthorized;

        var chosenOptions = await _appDbContext.OptionsForQuestions
            .Where(o => dto.ChosenOptionIds.Contains(o.OptionForQuestionId))
            .ToListAsync();

        var answer = new Answer
        {
            CorrespondingForm = form,
            ChosenOptions = chosenOptions,
            UserId = user.Id,
            Status = AnswerStatus.Saved
        };

        _appDbContext.Answers.Add(answer);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
            return ErrorCodes.Ok;
        return ErrorCodes.BadRequest;
    }
}