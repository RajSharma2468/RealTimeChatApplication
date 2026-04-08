namespace ConnectHub.Media.DTOs
{
    // Response DTO for media file information
    public class MediaResponseDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedAt { get; set; }
        public int? MessageId { get; set; }
        public int? RoomId { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}