using ConnectHub.Notification.Models;
using ConnectHub.Notification.DTOs;

namespace ConnectHub.Notification.Tests.Helpers
{
    public static class TestDataBuilder
    {
        public static NotificationEntity CreateTestNotification(int recipientId = 1, string type = "MESSAGE", string title = "Test", string message = "Test message")
        {
            return new NotificationEntity
            {
                RecipientId = recipientId,
                Type = type,
                Title = title,
                Message = message,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };
        }
        
        public static SendNotificationDto CreateSendNotificationDto(int recipientId = 2)
        {
            return new SendNotificationDto
            {
                RecipientId = recipientId,
                Type = "MESSAGE",
                Title = "New Message",
                Message = "You have a new message"
            };
        }
    }
}
