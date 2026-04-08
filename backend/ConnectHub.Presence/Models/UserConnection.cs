namespace ConnectHub.Presence.Models
{
    // Tracks a single WebSocket connection from a user
    // User can have multiple connections (mobile + desktop)
    public class UserConnection
    {
        public int UserId { get; set; }           // Which user
        public string ConnectionId { get; set; }  // SignalR connection ID
        public DateTime ConnectedAt { get; set; } // When connected
        public DateTime LastHeartbeat { get; set; } // Last ping from client
    }
}