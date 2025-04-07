using backend.Interfaces;
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
    public async Task<IActionResult> ChangeMyPreferences()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DisplayProfile()
    {
        throw new NotImplementedException();
    }
}
