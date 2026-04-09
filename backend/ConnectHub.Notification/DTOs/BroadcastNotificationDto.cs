using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Notification.DTOs
{
    public class BroadcastNotificationDto
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Message { get; set; }
    }
}
