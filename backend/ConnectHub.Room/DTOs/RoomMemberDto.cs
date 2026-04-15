using System;

namespace ConnectHub.Room.DTOs
{
    // ================================================================
    // ROOM MEMBER DTO - For returning room member information
    // ================================================================
    public class RoomMemberDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}