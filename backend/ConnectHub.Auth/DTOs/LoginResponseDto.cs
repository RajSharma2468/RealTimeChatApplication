namespace ConnectHub.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}