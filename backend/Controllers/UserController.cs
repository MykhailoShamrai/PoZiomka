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
    [Authorize]
    [Route("change-user-preferences")]
    public async Task<IActionResult> ChangeMyPreferences([FromBody] UserPreferences userPreferences)
    {
        var code = await _userInterface.ChangeUserPreferences(userPreferences);
        switch (code)
        {
            case UserErrorCodes.UserNotFound:
                return NotFound("User not found.");
            case UserErrorCodes.CannotRetrieveUserFromCookie:
                return Unauthorized("Cookie retrieval failed.");
            case UserErrorCodes.UpdateUserDbFailed:
                return BadRequest("Db update failed.");
            case UserErrorCodes.Ok:
                return Ok();
            default:
                throw new KeyNotFoundException();
        }
    }

    [HttpGet]
    [Authorize]
    [Route("display-user-profile/{email}")]
    public async Task<IActionResult> DisplayProfile([FromRoute] string email)
    {
        var (code, profile) = await _userInterface.DisplayUserProfile(email);
        switch (code)
        {
          case UserErrorCodes.UserNotFound:
              return NotFound("User not found.");
          case UserErrorCodes.Ok:
              return Ok(profile);
          default:
              throw new KeyNotFoundException();
        }
    }
}
