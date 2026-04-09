using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Notification.Models
{
    public class NotificationEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int RecipientId { get; set; }
        
        public int? SenderId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }
        
        public int? RelatedId { get; set; }
        
        [MaxLength(50)]
        public string? RelatedType { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        [Required]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
