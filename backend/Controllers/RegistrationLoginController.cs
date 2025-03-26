using backend.Models;
using backend.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class RegistrationLoginController : ControllerBase
{
    //private readonly SecurityService _securityService;

    [HttpPost]
    [Route("register-new-user")]
    public async Task<IActionResult> Register([FromBody] DtoRegisterUser model)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] DtoLoginUser model)
    {
        throw new NotImplementedException();
    }
}