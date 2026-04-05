namespace ConnectHub.Messaging.DTOs
{
    public class SearchMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime SentAt { get; set; }
        public string ConversationWith { get; set; }
    }
}
