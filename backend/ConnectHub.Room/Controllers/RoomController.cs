using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ConnectHub.Room.DTOs;
using ConnectHub.Room.Services;

namespace ConnectHub.Room.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        
        // Helper: Get current user ID from JWT token
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User not authenticated");
            return int.Parse(userIdClaim);
        }
        
        // POST: api/room/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var room = await _roomService.CreateRoomAsync(userId, dto);
                return Ok(new { success = true, data = room, message = "Room created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // GET: api/room/{roomId}
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var room = await _roomService.GetRoomByIdAsync(roomId, userId);
                return Ok(new { success = true, data = room });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }
        
        // GET: api/room/public
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicRooms()
        {
            var userId = GetCurrentUserId();
            var rooms = await _roomService.GetPublicRoomsAsync(userId);
            return Ok(new { success = true, data = rooms });
        }
        
        // GET: api/room/my-rooms
        [HttpGet("my-rooms")]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = GetCurrentUserId();
            var rooms = await _roomService.GetMyRoomsAsync(userId);
            return Ok(new { success = true, data = rooms });
        }
        
        // POST: api/room/join/{roomId}
        [HttpPost("join/{roomId}")]
        public async Task<IActionResult> JoinRoom(int roomId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _roomService.JoinRoomAsync(roomId, userId);
                return Ok(new { success = true, message = "Joined room successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // POST: api/room/leave/{roomId}
        [HttpPost("leave/{roomId}")]
        public async Task<IActionResult> LeaveRoom(int roomId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _roomService.LeaveRoomAsync(roomId, userId);
                return Ok(new { success = true, message = "Left room successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET: api/room/{roomId}/members - Get room members with real names
        // ================================================================
        [HttpGet("{roomId}/members")]
        public async Task<IActionResult> GetRoomMembers(int roomId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var members = await _roomService.GetRoomMembersAsync(roomId, userId, token);
                return Ok(new { success = true, data = members });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // PUT: api/room/member-role
        [HttpPut("member-role")]
        public async Task<IActionResult> UpdateMemberRole([FromBody] UpdateMemberRoleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _roomService.UpdateMemberRoleAsync(userId, dto);
                return Ok(new { success = true, message = "Member role updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // DELETE: api/room/{roomId}/member/{userId} - Remove member from room
        [HttpDelete("{roomId}/member/{userId}")]
        public async Task<IActionResult> RemoveMember(int roomId, int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var result = await _roomService.RemoveMemberAsync(roomId, userId, currentUserId);
                
                if (result)
                    return Ok(new { success = true, message = "Member removed successfully" });
                else
                    return BadRequest(new { success = false, message = "Failed to remove member" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // DELETE: api/room/{roomId}
        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _roomService.DeleteRoomAsync(roomId, userId);
                return Ok(new { success = true, message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}