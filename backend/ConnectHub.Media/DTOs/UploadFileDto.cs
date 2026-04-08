namespace ConnectHub.Media.DTOs
{
    // DTO for file upload response
    public class UploadFileDto
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}