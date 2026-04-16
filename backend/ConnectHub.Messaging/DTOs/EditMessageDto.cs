using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.DTOs
{
    public class EditMessageDto
    {
        public int MessageId { get; set; }
        
        [MinLength(1)]
        [MaxLength(5000)]
        public string? NewContent { get; set; }
    }
}