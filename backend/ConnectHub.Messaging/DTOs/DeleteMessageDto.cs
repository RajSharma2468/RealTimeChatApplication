using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.DTOs
{
    public class DeleteMessageDto
    {
        [Required]
        public int MessageId { get; set; }
        
        public string DeleteType { get; set; } = "FOR_ME";
    }
}