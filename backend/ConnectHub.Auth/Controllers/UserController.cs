using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Services;

namespace ConnectHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        // Dependency Injection
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await _userService.GetProfileAsync(userId);
            return Ok(new { success = true, data = profile });
        }
        
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var updated = await _userService.UpdateProfileAsync(userId, updateDto);
            return Ok(new { success = true, data = updated, message = "Profile updated successfully" });
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return Ok(new { success = true, data = new List<SearchUserDto>() });
            
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var results = await _userService.SearchUsersAsync(q, currentUserId);
            return Ok(new { success = true, data = results });
        }
    }
}