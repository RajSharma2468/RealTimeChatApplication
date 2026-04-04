using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Auth.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string DisplayName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}