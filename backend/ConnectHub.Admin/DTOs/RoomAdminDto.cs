namespace ConnectHub.Admin.DTOs
{
    // Room data for admin view
    public class RoomAdminDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public int MessageCount { get; set; }
        public bool IsActive { get; set; }
    }
}