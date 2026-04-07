using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.DTOs
{
    public class SendMessageDto
    {
        [Required]
        public int ReceiverId { get; set; }
        
        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Content { get; set; }
    }
}
