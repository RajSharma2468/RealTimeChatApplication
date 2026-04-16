using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SenderId { get; set; }
        
        // Store sender's display name for quick display (avoids extra API call)
        public string SenderName { get; set; } = string.Empty;
        
        public int? ReceiverId { get; set; }
        
        public int? RoomId { get; set; }
        
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; }
        
        // Global delete (for everyone)
        public bool IsDeleted { get; set; } = false;
        
        // Per-user delete flags
        public bool IsDeletedForSender { get; set; } = false;
        public bool IsDeletedForReceiver { get; set; } = false;
        
        public bool IsEdited { get; set; } = false;
        
        public DateTime? EditedAt { get; set; }
        
        public string? MediaUrl { get; set; }
        
        public string? MessageType { get; set; } = "TEXT";
    }
}