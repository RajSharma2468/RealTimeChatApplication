using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Services;

namespace ConnectHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Dependency Injection
        private readonly IUserService _userService;
        
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _userService.RegisterAsync(registerDto);
                return Ok(new { success = true, data = result, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(loginDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            await _userService.LogoutAsync(userId);
            return Ok(new { success = true, message = "Logout successful" });
        }
    }
}