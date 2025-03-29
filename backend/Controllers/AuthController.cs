using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    //private readonly SecurityService _securityService;
    private readonly IAuthInterface _authInterface;

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        Console.WriteLine($"Received registration request:");
        Console.WriteLine($"Email: {model.Email}");
        Console.WriteLine($"Password: {model.Password}");

        if (!await _authInterface.Register())
            return BadRequest();
        return Ok();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto model)
    {
        if (!await _authInterface.Login())
            return BadRequest();
        return Ok();
    }
}