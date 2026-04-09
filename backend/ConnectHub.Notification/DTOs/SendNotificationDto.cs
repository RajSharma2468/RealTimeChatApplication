using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Notification.DTOs
{
    public class SendNotificationDto
    {
        [Required]
        public int RecipientId { get; set; }
        
        public int? SenderId { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public int? RelatedId { get; set; }
        
        public string? RelatedType { get; set; }
    }
}
