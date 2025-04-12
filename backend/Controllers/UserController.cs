using System.Security.Claims;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserInterface _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserController(IUserInterface userInterface, 
                          UserManager<User> userManager,
                          AppDbContext appDbContext,
                          IHttpContextAccessor contextAccessor)
    {
        _userRepository = userInterface;
        _contextAccessor = contextAccessor;
        _userManager = userManager;
        _appDbContext = appDbContext;
    }
    
    [HttpPost]
    [Authorize]
    [Route("preferences")]
    public async Task<IActionResult> ChangeMyPreferences([FromBody] UserPreferences userPreferences)
    {
        var code = await _userRepository.ChangeUserPreferences(userPreferences);
        switch (code)
        {
            case ErrorCodes.NotFound:
                return NotFound("User not found.");
            case ErrorCodes.CannotRetrieveUserFromCookie:
                return Unauthorized("Cookie retrieval failed.");
            case ErrorCodes.UpdateUserDbFailed:
                return BadRequest("Db update failed.");
            case ErrorCodes.Ok:
                return Ok();
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("profile")]
    public async Task<IActionResult> DisplayProfile()
    {
        var email = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        if (email == null) {
            return Unauthorized();
        }
        var (code, profile) = await _userRepository.DisplayUserProfile(email);
        switch (code)
        {
          case ErrorCodes.NotFound:
              return NotFound("User not found.");
          case ErrorCodes.Ok:
              return Ok(profile);
          default:
              throw new KeyNotFoundException();
        }
    }

    [HttpPut]
    [Authorize]
    [Route("profile")]
    public async Task<IActionResult> ChangeProfile([FromBody] UpdateUserDto user)
    {
        var code = await _userRepository.ChangeUserProfile(user);
        switch (code)
        {
          case ErrorCodes.Forbidden:
              return NotFound("Forbidden");
          case ErrorCodes.Ok:
              return Ok();
          default:
              throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("forms")]
    public async Task<IActionResult> GetForms()
    {
        var email = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
        if (email == null) {
            return Unauthorized();
        }
        var (code, forms) = await _userRepository.GetUserForms(email);
        switch (code)
        {
          case ErrorCodes.NotFound:
              return NotFound("Forms not found.");
          case ErrorCodes.Ok:
              return Ok(forms);
          default:
              throw new KeyNotFoundException();
        }
    }

    [HttpPost]
    [Authorize]
    [Route("submit-answer")]
    public async Task<IActionResult> SubmitAnswer([FromBody] AnswerDto answerDto)
    {
        var email = _contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Unauthorized();
        }

        // Fetch the form
        var form = await _appDbContext.Forms
            .FirstOrDefaultAsync(f => f.FormId == answerDto.FormId);
        if (form == null)
        {
            return BadRequest("Form not found.");
        }

        var chosenOptions = await _appDbContext.OptionsForQuestions
            .Where(o => answerDto.ChosenOptionIds.Contains(o.OptionForQuestionId))
            .ToListAsync();

        var answer = new Answer
        {
            CorrespondingForm = form,
            UserId = answerDto.UserId,
            ChosenOptions = chosenOptions,
            Status = answerDto.Status
        };

        _appDbContext.Answers.Add(answer);
        var res = await _appDbContext.SaveChangesAsync();
        if (res > 0)
        {
            return Ok();
        }
        return BadRequest("Failed to save answer.");
    }
}
