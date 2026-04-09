namespace ConnectHub.Notification.DTOs
{
    // Event DTO for RabbitMQ messaging
    public class NotificationEventDto
    {
        public string EventType { get; set; }  // "NEW_MESSAGE", "MENTION", "OFFLINE_NOTIFICATION"
        public int RecipientId { get; set; }
        public int? SenderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? RelatedId { get; set; }
        public string RelatedType { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}