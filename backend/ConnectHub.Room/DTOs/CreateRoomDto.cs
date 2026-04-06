using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Room.DTOs
{
    // Data Transfer Object for creating a room
    // WHY DTO: To receive only required fields from the client, do not expose the Entity
    public class CreateRoomDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string RoomName { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public string RoomType { get; set; } = "PUBLIC";  // PUBLIC or PRIVATE
    }
}