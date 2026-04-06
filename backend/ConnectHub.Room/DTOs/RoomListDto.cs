namespace ConnectHub.Room.DTOs
{
    // Simplified DTO for room listing (without member details)
    public class RoomListDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? Description { get; set; }
        public string RoomType { get; set; }
        public int MemberCount { get; set; }
        public bool IsMember { get; set; }
        public string? UserRole { get; set; }
    }
}