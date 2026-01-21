using LibraryManagement.DTOs;
using LibraryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(loginDto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Register(registerDto);
            if (result == null)
                return BadRequest(new { message = "User with this email already exists" });

            return Ok(result);
        }
    }
}
