using ConnectHub.Admin.Models;
using ConnectHub.Admin.DTOs;

namespace ConnectHub.Admin.Tests.Helpers
{
    public static class TestDataBuilder
    {
        // Create test audit log
        public static AuditLog CreateTestAuditLog(
            int id = 0,
            int adminId = 1,
            string action = "SUSPEND_USER",
            string targetType = "USER",
            int targetId = 2)
        {
            return new AuditLog
            {
                Id = id,
                AdminId = adminId,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Details = null,
                IpAddress = "127.0.0.1",
                CreatedAt = DateTime.UtcNow
            };
        }
        
        // Create test user admin DTO
        public static UserAdminDto CreateTestUserAdminDto(
            int id = 1,
            string username = "admin",
            string displayName = "Admin User",
            bool isActive = true)
        {
            return new UserAdminDto
            {
                Id = id,
                Username = username,
                DisplayName = displayName,
                Email = $"{username}@example.com",
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastSeen = DateTime.UtcNow,
                MessageCount = 500
            };
        }
        
        // Create test room admin DTO
        public static RoomAdminDto CreateTestRoomAdminDto(
            int id = 1,
            string roomName = "Test Room",
            bool isActive = true)
        {
            return new RoomAdminDto
            {
                Id = id,
                RoomName = roomName,
                RoomType = "PUBLIC",
                CreatedBy = 1,
                CreatorName = "Admin",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                MemberCount = 25,
                MessageCount = 500,
                IsActive = isActive
            };
        }
        
        // Create test message admin DTO
        public static MessageAdminDto CreateTestMessageAdminDto(
            int id = 1,
            int senderId = 2,
            string content = "Test message")
        {
            return new MessageAdminDto
            {
                Id = id,
                SenderId = senderId,
                SenderName = $"User_{senderId}",
                ReceiverId = 1,
                ReceiverName = "Admin",
                RoomId = null,
                RoomName = null,
                Content = content,
                SentAt = DateTime.UtcNow.AddHours(-1),
                IsDeleted = false
            };
        }
        
        // Create test analytics DTO
        public static AnalyticsDto CreateTestAnalyticsDto()
        {
            return new AnalyticsDto
            {
                TotalUsers = 150,
                ActiveUsers24h = 45,
                TotalMessages = 12500,
                MessagesToday = 342,
                TotalRooms = 25,
                ActiveConnections = 38,
                GeneratedAt = DateTime.UtcNow
            };
        }
        
        // Create list of users
        public static List<UserAdminDto> CreateMultipleUsers(int count = 5)
        {
            var users = new List<UserAdminDto>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(CreateTestUserAdminDto(id: i, username: $"user{i}", isActive: i % 2 == 0));
            }
            return users;
        }
        
        // Create list of rooms
        public static List<RoomAdminDto> CreateMultipleRooms(int count = 5)
        {
            var rooms = new List<RoomAdminDto>();
            for (int i = 1; i <= count; i++)
            {
                rooms.Add(CreateTestRoomAdminDto(id: i, roomName: $"Room {i}"));
            }
            return rooms;
        }
        
        // Create list of messages
        public static List<MessageAdminDto> CreateMultipleMessages(int count = 5)
        {
            var messages = new List<MessageAdminDto>();
            for (int i = 1; i <= count; i++)
            {
                messages.Add(CreateTestMessageAdminDto(id: i, content: $"Message {i}"));
            }
            return messages;
        }
    }
}