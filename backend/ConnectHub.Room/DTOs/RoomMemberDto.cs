namespace ConnectHub.Room.DTOs
{
    
    // RoomMemberDto - Data transfer object for room member information
    
    public class RoomMemberDto
    {
        public int UserId { get; set; }      // User ID of the member
        public string UserName { get; set; } // Display name of the member
        public string Role { get; set; }     // ADMIN or MEMBER
        public DateTime JoinedAt { get; set; } // When the user joined
    }
}