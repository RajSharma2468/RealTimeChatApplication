namespace ConnectHub.Auth.DTOs
{
    public class SearchUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsOnline { get; set; }
    }
}