using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserInterface _userInterface;

    public UserController(IUserInterface userInterface)
    {
        _userInterface = userInterface;
    }
    
    [HttpPost]
    [Authorize("Student")]
    [Route("change-display-preferences")]
    public async Task<IActionResult> ChangeMyPreferences([FromBody] bool[] displayPreferences)
    {
        var code = await _userInterface.ChangeUserPreferences(displayPreferences);
        switch (code)
        {
            case UserErrorCodes.UserNotFound:
                return NotFound("User not found.");
            case UserErrorCodes.PreferencesNotFound:
                return BadRequest("Preferences not found.");
            case UserErrorCodes.Ok:
                return Ok();
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("display-user-profile")]
    public async Task<IActionResult> DisplayProfile([FromBody] string email)
    {
        var (code, profile) = await _userInterface.DisplayUserProfile(email);
        switch (code)
        {
          case UserErrorCodes.UserNotFound:
              return NotFound("User not found.");
          case UserErrorCodes.PreferencesNotFound:
              return BadRequest("Preferences not found.");
          case UserErrorCodes.Ok:
              return Ok(profile);
          default:
              throw new KeyNotFoundException();
        }
    }
}
