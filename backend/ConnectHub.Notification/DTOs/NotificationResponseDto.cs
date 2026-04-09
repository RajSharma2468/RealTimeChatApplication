namespace ConnectHub.Notification.DTOs
{
    public class NotificationResponseDto
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public int? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? RelatedId { get; set; }
        public string? RelatedType { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}
