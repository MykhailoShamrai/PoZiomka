using backend.Dto;
using backend.Interfaces;
using backend.Models.User;
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
        throw new NotImplementedException();
    }

    [HttpGet]
    [Authorize]
    [Route("display-user-profile")]
    public async Task<IActionResult> DisplayProfile([FromBody] string email)
    {
        throw new NotImplementedException();
    }
}
