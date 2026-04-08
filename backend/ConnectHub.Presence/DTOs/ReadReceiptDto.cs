namespace ConnectHub.Presence.DTOs
{
    // Sent when user reads a message
    public class ReadReceiptDto
    {
        public int MessageId { get; set; }   // Which message was read
        public int ReaderId { get; set; }    // Who read it
        public int SenderId { get; set; }    // Original sender (to notify)
        public DateTime ReadAt { get; set; } // When it was read
    }
}