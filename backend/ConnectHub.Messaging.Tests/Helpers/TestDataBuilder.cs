using ConnectHub.Messaging.Models;
using ConnectHub.Messaging.DTOs;

namespace ConnectHub.Messaging.Tests.Helpers
{
    /// Creates test data for messaging tests

    public static class TestDataBuilder
    {
        // ========== MESSAGE BUILDER ==========
        
        public static Message CreateTestMessage(int id = 0, int senderId = 1, int receiverId = 2, 
            string content = "Test message", bool isRead = false)
        {
            return new Message
            {
                Id = id,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = isRead,
                IsDeleted = false,
                IsEdited = false,
                MessageType = "TEXT"
            };
        }
        
        public static Message CreateEditedMessage(int id, string newContent)
        {
            var message = CreateTestMessage(id: id, content: "Original content");
            message.Content = newContent;
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;
            return message;
        }
        
        public static Message CreateDeletedMessage(int id)
        {
            var message = CreateTestMessage(id: id);
            message.IsDeleted = true;
            message.Content = "[Message deleted]";
            return message;
        }
        
        // ========== DTO BUILDER ==========
        
        public static SendMessageDto CreateSendMessageDto(int receiverId = 2, string content = "Hello!")
        {
            return new SendMessageDto
            {
                ReceiverId = receiverId,
                Content = content
            };
        }
        
        public static EditMessageDto CreateEditMessageDto(int messageId = 1, string newContent = "Updated content")
        {
            return new EditMessageDto
            {
                MessageId = messageId,
                NewContent = newContent
            };
        }
        
        // ========== MULTIPLE MESSAGES ==========
        
        public static List<Message> CreateConversationHistory(int count = 10)
        {
            var messages = new List<Message>();
            for (int i = 1; i <= count; i++)
            {
                messages.Add(CreateTestMessage(id: i, senderId: i % 2 == 0 ? 2 : 1, 
                    receiverId: i % 2 == 0 ? 1 : 2, content: $"Message {i}"));
            }
            return messages;
        }
        
        // ========== ROOM MESSAGES ==========
        
        public static Message CreateRoomMessage(int roomId = 1, int senderId = 1, string content = "Room message")
        {
            return new Message
            {
                Id = 0,
                SenderId = senderId,
                RoomId = roomId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                MessageType = "TEXT"
            };
        }
    }
}