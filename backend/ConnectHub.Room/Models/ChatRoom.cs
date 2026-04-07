using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectHub.Room.Models
{
    // ChatRoom entity - Represents a group chat room
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string RoomName { get; set; }  // Name of the room (e.g., "Cricket Fans")
        
        [MaxLength(500)]
        public string? Description { get; set; }  // Room description
        
        [Required]
        public string RoomType { get; set; } = "PUBLIC";  // PUBLIC, PRIVATE, DIRECT
        
        public string? AvatarUrl { get; set; }  // Room avatar image
        
        [Required]
        public int CreatedBy { get; set; }  // User ID who created the room
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;  // Soft delete flag
        
        public int MaxMembers { get; set; } = 500;  // Maximum members limit
    }
}