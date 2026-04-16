using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs;
using RetailAPI.Services;

namespace RetailAPI.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var success = await _authService.RegisterAsync(request);
            if (!success) return BadRequest("Email already exists.");

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null) return Unauthorized("Invalid credentials.");

            return Ok(response);
        }

        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var authService = (AuthService)_authService; // Casting to access internal DbContext for quick demo
            
            // In a real app we'd add this to the Interface properly
            return await authService.GetProfileAsync(userId) switch {
                null => NotFound(),
                var profile => Ok(profile)
            };
        }
    }
}
