using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Admin.Models
{
    // AuditLog entity - Tracks all admin actions
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Action { get; set; }  // SUSPEND_USER, DELETE_ROOM, DELETE_MESSAGE, etc.
        
        [Required]
        [MaxLength(50)]
        public string TargetType { get; set; }  // USER, ROOM, MESSAGE
        
        public int TargetId { get; set; }
        
        [MaxLength(500)]
        public string? Details { get; set; }  // JSON details
        
        [MaxLength(50)]
        public string? IpAddress { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}