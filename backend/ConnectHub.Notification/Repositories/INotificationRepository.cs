using ConnectHub.Notification.Models;

namespace ConnectHub.Notification.Repositories
{
    public interface INotificationRepository
    {
        Task<NotificationEntity> CreateAsync(NotificationEntity notification);
        Task<NotificationEntity?> GetByIdAsync(int id);
        Task<IEnumerable<NotificationEntity>> GetByRecipientAsync(int recipientId, int page = 1, int pageSize = 20);
        Task<IEnumerable<NotificationEntity>> GetUnreadByRecipientAsync(int recipientId);
        Task<int> GetUnreadCountAsync(int recipientId);
        Task<bool> MarkAsReadAsync(int id, int recipientId);
        Task<bool> MarkAllAsReadAsync(int recipientId);
        Task<bool> DeleteAsync(int id, int recipientId);
    }
}
