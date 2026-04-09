using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Admin.Services;
using ConnectHub.Admin.DTOs;

namespace ConnectHub.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]  // Only admin can access
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        
        // ========== USER MANAGEMENT ==========
        
        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(new { success = true, data = users });
        }
        
        // GET: api/admin/users/{id}
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });
            
            return Ok(new { success = true, data = user });
        }
        
        // PUT: api/admin/users/{id}/suspend
        [HttpPut("users/{id}/suspend")]
        public async Task<IActionResult> SuspendUser(int id, [FromBody] string reason)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.SuspendUserAsync(adminId, id, reason);
            
            if (!result)
                return BadRequest(new { success = false, message = "Failed to suspend user" });
            
            return Ok(new { success = true, message = "User suspended successfully" });
        }
        
        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.DeleteUserAsync(adminId, id);
            
            if (!result)
                return BadRequest(new { success = false, message = "Failed to delete user" });
            
            return Ok(new { success = true, message = "User deleted successfully" });
        }
        
        // ========== ROOM MANAGEMENT ==========
        
        // GET: api/admin/rooms
        [HttpGet("rooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _adminService.GetAllRoomsAsync();
            return Ok(new { success = true, data = rooms });
        }
        
        // DELETE: api/admin/rooms/{id}
        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.DeleteRoomAsync(adminId, id);
            
            if (!result)
                return BadRequest(new { success = false, message = "Failed to delete room" });
            
            return Ok(new { success = true, message = "Room deleted successfully" });
        }
        
        // ========== MESSAGE MANAGEMENT ==========
        
        // GET: api/admin/messages
        [HttpGet("messages")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _adminService.GetAllMessagesAsync();
            return Ok(new { success = true, data = messages });
        }
        
        // DELETE: api/admin/messages/{id}
        [HttpDelete("messages/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.DeleteMessageAsync(adminId, id);
            
            if (!result)
                return BadRequest(new { success = false, message = "Failed to delete message" });
            
            return Ok(new { success = true, message = "Message deleted successfully" });
        }
        
        // ========== ANALYTICS ==========
        
        // GET: api/admin/analytics
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var analytics = await _adminService.GetAnalyticsAsync();
            return Ok(new { success = true, data = analytics });
        }
        
        // ========== AUDIT LOGS ==========
        
        // GET: api/admin/audit-logs
        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await _adminService.GetAuditLogsAsync(page, pageSize);
            return Ok(new { success = true, data = logs });
        }
    }
}