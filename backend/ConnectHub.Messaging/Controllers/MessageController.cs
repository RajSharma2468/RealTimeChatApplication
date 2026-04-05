using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Services;

namespace ConnectHub.Messaging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // All endpoints require JWT token
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        // GET: api/message/direct/5?page=1&pageSize=20
        [HttpGet("direct/{userId}")]
        public async Task<IActionResult> GetDirectMessages(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var messages = await _messageService.GetDirectMessagesAsync(currentUserId, userId, page, pageSize);
            return Ok(new { success = true, data = messages });
        }
        
        // GET: api/message/room/5?page=1&pageSize=20
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRoomMessages(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var messages = await _messageService.GetRoomMessagesAsync(roomId, page, pageSize);
            return Ok(new { success = true, data = messages });
        }
        
        // POST: api/message/send
        [HttpPost("send")]
        public async Task<IActionResult> SendDirectMessage([FromBody] SendMessageDto dto)
        {
            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var message = await _messageService.SendDirectMessageAsync(senderId, dto);
            return Ok(new { success = true, data = message, message = "Message sent successfully" });
        }
        
        // PUT: api/message/edit
        [HttpPut("edit")]
        public async Task<IActionResult> EditMessage([FromBody] EditMessageDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var edited = await _messageService.EditMessageAsync(userId, dto);
            return Ok(new { success = true, data = edited, message = "Message edited successfully" });
        }
        
        // DELETE: api/message/5
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _messageService.DeleteMessageAsync(userId, messageId);
            return Ok(new { success = true, message = "Message deleted successfully" });
        }
        
        // GET: api/message/search?q=hello&roomId=5
        [HttpGet("search")]
        public async Task<IActionResult> SearchMessages([FromQuery] string q, [FromQuery] int? roomId = null)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return Ok(new { success = true, data = new List<SearchMessageDto>() });
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var results = await _messageService.SearchMessagesAsync(userId, q, roomId);
            return Ok(new { success = true, data = results });
        }
        
        // GET: api/message/unread/count
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var count = await _messageService.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = count });
        }
        
        // PUT: api/message/read/5
        [HttpPut("read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _messageService.MarkAsReadAsync(userId, messageId);
            return Ok(new { success = true, message = "Message marked as read" });
        }
        
        // PUT: api/message/read-all?senderId=5
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] int? senderId = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _messageService.MarkAllAsReadAsync(userId, senderId);
            return Ok(new { success = true, message = "All messages marked as read" });
        }
        
        // GET: api/message/recent-chats
        [HttpGet("recent-chats")]
        public async Task<IActionResult> GetRecentChats()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var chats = await _messageService.GetRecentChatsAsync(userId);
            return Ok(new { success = true, data = chats });
        }
    }
}
