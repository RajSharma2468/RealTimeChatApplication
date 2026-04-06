using System.ComponentModel.DataAnnotations;

namespace ConnectHub.Room.DTOs
{
    // DTO for updating member role (Admin only)
    public class UpdateMemberRoleDto
    {
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public string NewRole { get; set; }  // ADMIN or MEMBER
    }
}