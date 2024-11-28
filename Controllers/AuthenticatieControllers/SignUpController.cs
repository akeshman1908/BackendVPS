using backend.Domain;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.AuthenticatieControllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class SignUpController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SignUpController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var result = await _authService.SignUpAsync(new User
            {
                username = request.Username,
                email = request.Email,
                password = request.Password, // Wachtwoord moet worden gehasht in de service
                rolid = request.RoleId
            });

            if (!result)
                return BadRequest("Unable to register user");

            return Ok("User registered successfully");
        }
    }
}

public class SignUpRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}