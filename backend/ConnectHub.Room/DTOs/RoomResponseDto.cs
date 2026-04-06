namespace ConnectHub.Room.DTOs
{
    // Response DTO - Sends room data to client
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? Description { get; set; }
        public string RoomType { get; set; }
        public string? AvatarUrl { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }  // Will be populated from Auth service
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public string UserRole { get; set; }  // ADMIN or MEMBER for current user
    }
}