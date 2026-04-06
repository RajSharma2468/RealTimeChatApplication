using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectHub.Room.Models
{
    // RoomMember entity - Tracks which user is in which room
    public class RoomMember
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int RoomId { get; set; }  // Which room
        
        [Required]
        public int UserId { get; set; }  // Which user
        
        [Required]
        public string Role { get; set; } = "MEMBER";  // ADMIN, MODERATOR, MEMBER
        
        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;  // Soft delete flag
    }
}