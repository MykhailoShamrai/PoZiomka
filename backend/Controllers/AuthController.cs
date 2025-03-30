    using backend.Dto;
    using backend.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace backend.Controllers;

    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //private readonly SecurityService _securityService;
        private readonly IAuthInterface _authInterface;

        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            Console.WriteLine($"Received registration request:");
            Console.WriteLine($"Email: {model.Email}");
            Console.WriteLine($"Password: {model.Password}");

            if (!await _authInterface.Register(model))
                return BadRequest();
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            if (!await _authInterface.Login(model))
                return BadRequest();
            return Ok();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authInterface.Logout();
            return Ok();
        }

        [HttpGet]
        [Authorize("Admin")]
        [Route("test_for_admin")]
        public async Task<IActionResult> Test()
        {
            if (User.Identity is not { IsAuthenticated: true })
            {
                return Unauthorized("User is not authenticated!");
            }
            return Ok();
        }
    }