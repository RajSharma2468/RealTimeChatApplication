namespace ConnectHub.Admin.DTOs
{
    // User data for admin view
    public class UserAdminDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSeen { get; set; }
        public int MessageCount { get; set; }
    }
}