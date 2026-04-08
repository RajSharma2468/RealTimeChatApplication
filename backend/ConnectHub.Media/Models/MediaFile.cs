using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectHub.Media.Models
{
    // MediaFile entity - Represents uploaded file in database
    public class MediaFile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FileName { get; set; }           // Original file name
        
        [Required]
        public string FilePath { get; set; }           // Path where file is stored
        
        [Required]
        public string ContentType { get; set; }        // image/jpeg, image/png, etc.
        
        [Required]
        public long FileSize { get; set; }             // Size in bytes
        
        [Required]
        public int UploadedBy { get; set; }            // User ID who uploaded
        
        public int? MessageId { get; set; }            // Associated message ID
        
        public int? RoomId { get; set; }               // For group chat files
        
        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpiresAt { get; set; }       // Auto-delete after this date
    }
}