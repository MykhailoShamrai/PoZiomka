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
    public async Task<IActionResult> ChangeMyPreferences([FromBody] DisplayPreferences displayPreferences)
    {
        var code = await _userInterface.ChangeUserPreferences(displayPreferences);
        switch (code)
        {
            case UserErrorCodes.UserNotFound:
                return NotFound("User not found.");
            default:
                return Ok();
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
          default:
              return Ok(profile);
        }
    }
}
