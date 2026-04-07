using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Auth.DTOs
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }
        
        [MaxLength(500)]
        public string? Bio { get; set; }
        
        public string? AvatarUrl { get; set; }
    }
}