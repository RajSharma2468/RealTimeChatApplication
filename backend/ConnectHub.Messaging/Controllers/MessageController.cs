using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Services;

namespace ConnectHub.Messaging.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
        }
        
        // ================================================================
        // SEND DIRECT MESSAGE - Supports /direct and /send
        // ================================================================
        [HttpPost("direct")]
        [HttpPost("send")]
        public async Task<IActionResult> SendDirectMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                var result = await _messageService.SendDirectMessageAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // SEND ROOM MESSAGE - Supports /room and /room/send
        // ================================================================
        [HttpPost("room")]
        [HttpPost("room/send")]
        public async Task<IActionResult> SendRoomMessage([FromBody] SendRoomMessageDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                var result = await _messageService.SendRoomMessageAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET RECENT CHATS - WITH TOKEN FOR AUTH SERVICE
        // ================================================================
        [HttpGet("recent-chats")]
        public async Task<IActionResult> GetRecentChats()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                // Extract token from Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;
                
                var recentChats = await _messageService.GetRecentChatsAsync(userId, token);
                return Ok(new { success = true, data = recentChats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET DIRECT MESSAGES BETWEEN TWO USERS
        // ================================================================
        [HttpGet("direct/{userId}")]
        public async Task<IActionResult> GetDirectMessages(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                var messages = await _messageService.GetDirectMessagesAsync(currentUserId, userId, page, pageSize);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET ROOM MESSAGES
        // ================================================================
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetRoomMessages(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                var messages = await _messageService.GetRoomMessagesAsync(roomId, page, pageSize);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // EDIT MESSAGE - POST endpoint
        // ================================================================
        [HttpPost("edit")]
        public async Task<IActionResult> EditMessagePost([FromBody] EditMessageDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                if (dto.MessageId == 0)
                    return BadRequest(new { success = false, message = "MessageId is required" });
                
                if (string.IsNullOrEmpty(dto.NewContent))
                    return BadRequest(new { success = false, message = "NewContent is required" });
                
                var result = await _messageService.EditMessageAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // EDIT MESSAGE - PUT endpoint
        // ================================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessagePut(int id, [FromBody] EditMessageDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                dto.MessageId = id;
                
                if (string.IsNullOrEmpty(dto.NewContent))
                    return BadRequest(new { success = false, message = "NewContent is required" });
                
                var result = await _messageService.EditMessageAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // DELETE MESSAGE - Supports both path and query parameter
        // ================================================================
        [HttpDelete("{id}")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMessage(int? id, [FromQuery] int? messageId, [FromQuery] string deleteType = "FOR_ME")
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                int finalMessageId = id ?? messageId ?? 0;
                
                if (finalMessageId == 0)
                    return BadRequest(new { success = false, message = "Message ID is required" });
                
                var dto = new DeleteMessageDto { MessageId = finalMessageId, DeleteType = deleteType };
                var result = await _messageService.DeleteMessageAsync(userId, dto);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // SEARCH MESSAGES
        // ================================================================
        [HttpGet("search")]
        public async Task<IActionResult> SearchMessages([FromQuery] string q, [FromQuery] int? roomId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                
                var results = await _messageService.SearchMessagesAsync(userId, q, roomId);
                return Ok(new { success = true, data = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET UNREAD COUNT
        // ================================================================
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { success = false, message = "User not authenticated" });
            
            var count = await _messageService.GetUnreadCountAsync(userId);
            return Ok(new { success = true, data = count });
        }
        
        // ================================================================
        // MARK MESSAGE AS READ - Supports both POST and PUT
        // ================================================================
        [HttpPost("read/{messageId}")]
        [HttpPut("read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { success = false, message = "User not authenticated" });
            
            var result = await _messageService.MarkAsReadAsync(userId, messageId);
            return Ok(new { success = result });
        }
        
        // ================================================================
        // MARK ALL AS READ
        // ================================================================
        [HttpPost("read/all")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] int? senderId = null)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { success = false, message = "User not authenticated" });
            
            var result = await _messageService.MarkAllAsReadAsync(userId, senderId);
            return Ok(new { success = result });
        }
    }
}