using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.DTOs
{
    public class EditMessageDto
    {
        [Required]
        public int MessageId { get; set; }
        
        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string NewContent { get; set; }
    }
}
