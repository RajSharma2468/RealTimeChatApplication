using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Messaging.DTOs
{
    public class SendRoomMessageDto
    {
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;
    }
}