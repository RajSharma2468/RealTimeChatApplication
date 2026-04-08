namespace ConnectHub.Presence.DTOs
{
    // Sent when user starts/stops typing in chat
    public class TypingIndicatorDto
    {
        public int SenderId { get; set; }      // Who is typing
        public int ReceiverId { get; set; }    // Who will receive the indicator
        public bool IsTyping { get; set; }     // true = started typing, false = stopped
        public string ConversationType { get; set; } // "DIRECT" or "ROOM"
        public int? RoomId { get; set; }       // For room chats
    }
}