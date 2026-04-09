namespace ConnectHub.Admin.DTOs
{
    // Audit log data transfer object
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string Action { get; set; }
        public string TargetType { get; set; }
        public int TargetId { get; set; }
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}