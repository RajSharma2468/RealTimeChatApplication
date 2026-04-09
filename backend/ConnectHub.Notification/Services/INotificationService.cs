using ConnectHub.Notification.DTOs;

namespace ConnectHub.Notification.Services
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> SendNotificationAsync(SendNotificationDto dto);
        Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(int userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<bool> BroadcastNotificationAsync(int adminUserId, BroadcastNotificationDto dto);
        
        // RabbitMQ methods
        Task PublishNotificationEventAsync(NotificationEventDto eventDto);
        Task ProcessQueueMessageAsync(string message);
    }
}