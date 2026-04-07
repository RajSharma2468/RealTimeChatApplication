using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SenderId { get; set; }
        
        public int? ReceiverId { get; set; }
        
        public int? RoomId { get; set; }
        
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; }
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public bool IsEdited { get; set; } = false;
        
        public DateTime? EditedAt { get; set; }
        
        public string? MediaUrl { get; set; }
        
        public string? MessageType { get; set; } = "TEXT";
    }
}
