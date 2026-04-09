namespace ConnectHub.Admin.DTOs
{
    // Message data for admin view
    public class MessageAdminDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}