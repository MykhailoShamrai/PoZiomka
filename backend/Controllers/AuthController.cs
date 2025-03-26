using backend.Dto;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    //private readonly SecurityService _securityService;

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        Console.WriteLine($"Received registration request:");
        Console.WriteLine($"Email: {model.Email}");
        Console.WriteLine($"Password: {model.HashedPassword}");
        
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto model)
    {
        throw new NotImplementedException();
    }
}