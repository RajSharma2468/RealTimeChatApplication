using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Notification.Services;
using ConnectHub.Notification.DTOs;

namespace ConnectHub.Notification.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetMyNotificationsAsync(userId, page, pageSize);
            return Ok(new { success = true, data = notifications });
        }
        
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = count });
        }
        
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(new { success = true, data = notifications });
        }
        
        [HttpPut("read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAsReadAsync(id, userId);
            
            if (!result)
                return NotFound(new { success = false, message = "Notification not found" });
            
            return Ok(new { success = true, message = "Notification marked as read" });
        }
        
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success = true, message = "All notifications marked as read" });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            
            if (!result)
                return NotFound(new { success = false, message = "Notification not found" });
            
            return Ok(new { success = true, message = "Notification deleted successfully" });
        }
        
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastNotificationDto dto)
        {
            var userId = GetCurrentUserId();
            await _notificationService.BroadcastNotificationAsync(userId, dto);
            return Ok(new { success = true, message = "Broadcast sent successfully" });
        }
    }
}
