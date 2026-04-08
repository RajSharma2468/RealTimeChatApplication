using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConnectHub.Presence.Services;

namespace ConnectHub.Presence.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PresenceController : ControllerBase
    {
        private readonly IPresenceService _presenceService;
        
        public PresenceController(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }
        
        // GET: api/presence/online - Get all online user IDs
        [HttpGet("online")]
        public IActionResult GetOnlineUsers()
        {
            var onlineUserIds = _presenceService.GetOnlineUserIds();
            return Ok(new { success = true, data = onlineUserIds });
        }
        
        // GET: api/presence/is-online/5 - Check if specific user is online
        [HttpGet("is-online/{userId}")]
        public IActionResult IsUserOnline(int userId)
        {
            var isOnline = _presenceService.IsUserOnline(userId);
            return Ok(new { success = true, data = isOnline });
        }
        
        // GET: api/presence/last-seen/5 - Get user's last seen time
        [HttpGet("last-seen/{userId}")]
        public IActionResult GetLastSeen(int userId)
        {
            var lastSeen = _presenceService.GetLastSeen(userId);
            return Ok(new { success = true, data = lastSeen });
        }
        
        // GET: api/presence/connection-count - Get total active connections
        [HttpGet("connection-count")]
        public IActionResult GetConnectionCount()
        {
            var count = _presenceService.GetConnectionCount();
            return Ok(new { success = true, data = count });
        }
    }
}