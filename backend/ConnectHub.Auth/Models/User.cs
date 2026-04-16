using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Auth.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string PasswordHash { get; set; }
        
        public string? Bio { get; set; }
        
        public string? AvatarUrl { get; set; }
        
        // ================================================================
        // GOOGLE ID - For Google OAuth authentication
        // Stores Google's unique user identifier
        // Null for users who registered normally
        // ================================================================
        public string? GoogleId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastSeen { get; set; }
        
        // ================================================================
        // ONLINE STATUS - For presence service
        // ================================================================
        public bool IsOnline { get; set; } = false;
        
        // ================================================================
        // USER ROLE - For authorization (ADMIN, USER)
        // ================================================================
        public string Role { get; set; } = "USER";
    }
}