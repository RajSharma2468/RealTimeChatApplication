using ConnectHub.Presence.Models;
using ConnectHub.Presence.DTOs;

namespace ConnectHub.Presence.Tests.Helpers
{
    // Creates test data for presence tests
    public static class TestDataBuilder
    {
        // Create UserConnection object
        public static UserConnection CreateUserConnection(
            int userId = 1, 
            string connectionId = "conn_001", 
            bool isActive = true)
        {
            return new UserConnection
            {
                UserId = userId,
                ConnectionId = connectionId,
                ConnectedAt = DateTime.UtcNow,
                LastHeartbeat = DateTime.UtcNow
            };
        }
        
        // Create stale connection (old heartbeat)
        public static UserConnection CreateStaleConnection(
            int userId = 1, 
            string connectionId = "conn_001",
            int staleSeconds = 120)
        {
            return new UserConnection
            {
                UserId = userId,
                ConnectionId = connectionId,
                ConnectedAt = DateTime.UtcNow.AddSeconds(-staleSeconds),
                LastHeartbeat = DateTime.UtcNow.AddSeconds(-staleSeconds)
            };
        }
        
        // Create TypingIndicatorDto
        public static TypingIndicatorDto CreateTypingIndicatorDto(
            int senderId = 1, 
            int receiverId = 2, 
            bool isTyping = true,
            string conversationType = "DIRECT")
        {
            return new TypingIndicatorDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                IsTyping = isTyping,
                ConversationType = conversationType,
                RoomId = conversationType == "ROOM" ? 1 : null
            };
        }
        
        // Create ReadReceiptDto
        public static ReadReceiptDto CreateReadReceiptDto(
            int messageId = 100, 
            int readerId = 2, 
            int senderId = 1)
        {
            return new ReadReceiptDto
            {
                MessageId = messageId,
                ReaderId = readerId,
                SenderId = senderId,
                ReadAt = DateTime.UtcNow
            };
        }
        
        // Create OnlineUserDto
        public static OnlineUserDto CreateOnlineUserDto(int userId = 1, string username = "testuser")
        {
            return new OnlineUserDto
            {
                UserId = userId,
                Username = username,
                DisplayName = $"User {username}",
                LastSeen = null
            };
        }
    }
}