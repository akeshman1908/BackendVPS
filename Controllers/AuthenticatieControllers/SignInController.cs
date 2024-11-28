using backend.Services.Interfaces;


using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers.AuthenticatieControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignInController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SignInController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var token = await _authService.SignInAsync(request.Username, request.Password);

            if (token == null)
                return Unauthorized("Invalid username or password");

            return Ok(new { Token = token });
        }
    }
}

public class SignInRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
