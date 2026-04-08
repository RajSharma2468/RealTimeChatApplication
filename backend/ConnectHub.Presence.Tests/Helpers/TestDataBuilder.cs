using ConnectHub.Presence.Models;
using ConnectHub.Presence.DTOs;

namespace ConnectHub.Presence.Tests.Helpers
{
    // Creates test data for presence tests
    public static class TestDataBuilder
    {
        // Create a test user connection
        public static UserConnection CreateUserConnection(
            int userId = TestConstants.UserId1,
            string connectionId = TestConstants.ConnectionId1,
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
        
        // Create a stale connection (heartbeat too old)
        public static UserConnection CreateStaleConnection(
            int userId = TestConstants.UserId1,
            string connectionId = TestConstants.ConnectionId1,
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
        
        // Create typing indicator DTO
        public static TypingIndicatorDto CreateTypingIndicatorDto(
            int senderId = TestConstants.UserId1,
            int receiverId = TestConstants.UserId2,
            bool isTyping = true,
            string conversationType = TestConstants.ConversationTypeDirect)
        {
            return new TypingIndicatorDto
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                IsTyping = isTyping,
                ConversationType = conversationType,
                RoomId = conversationType == TestConstants.ConversationTypeRoom ? TestConstants.RoomId : null
            };
        }
        
        // Create read receipt DTO
        public static ReadReceiptDto CreateReadReceiptDto(
            int messageId = TestConstants.MessageId,
            int readerId = TestConstants.UserId2,
            int senderId = TestConstants.UserId1)
        {
            return new ReadReceiptDto
            {
                MessageId = messageId,
                ReaderId = readerId,
                SenderId = senderId,
                ReadAt = DateTime.UtcNow
            };
        }
        
        // Create multiple connections for same user (multi-device)
        public static List<UserConnection> CreateMultipleConnections(int userId, int count = 3)
        {
            var connections = new List<UserConnection>();
            for (int i = 1; i <= count; i++)
            {
                connections.Add(CreateUserConnection(userId, $"conn_{userId}_{i}"));
            }
            return connections;
        }
    }
}