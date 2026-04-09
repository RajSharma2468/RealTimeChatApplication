using ConnectHub.Notification.Models;
using ConnectHub.Notification.Repositories;

namespace ConnectHub.Notification.Tests.Mocks
{
    public class MockNotificationRepository : INotificationRepository
    {
        private readonly List<NotificationEntity> _notifications = new();
        private int _nextId = 1;
        
        public Task<NotificationEntity> CreateAsync(NotificationEntity notification)
        {
            notification.Id = _nextId++;
            _notifications.Add(notification);
            return Task.FromResult(notification);
        }
        
        public Task<NotificationEntity?> GetByIdAsync(int id)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == id);
            return Task.FromResult(notification);
        }
        
        public Task<IEnumerable<NotificationEntity>> GetByRecipientAsync(int recipientId, int page = 1, int pageSize = 20)
        {
            var result = _notifications.Where(n => n.RecipientId == recipientId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return Task.FromResult(result.AsEnumerable());
        }
        
        public Task<IEnumerable<NotificationEntity>> GetUnreadByRecipientAsync(int recipientId)
        {
            var result = _notifications.Where(n => n.RecipientId == recipientId && !n.IsRead).ToList();
            return Task.FromResult(result.AsEnumerable());
        }
        
        public Task<int> GetUnreadCountAsync(int recipientId)
        {
            var count = _notifications.Count(n => n.RecipientId == recipientId && !n.IsRead);
            return Task.FromResult(count);
        }
        
        public Task<bool> MarkAsReadAsync(int id, int recipientId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == id && n.RecipientId == recipientId);
            if (notification == null) return Task.FromResult(false);
            notification.IsRead = true;
            return Task.FromResult(true);
        }
        
        public Task<bool> MarkAllAsReadAsync(int recipientId)
        {
            foreach (var n in _notifications.Where(n => n.RecipientId == recipientId)) n.IsRead = true;
            return Task.FromResult(true);
        }
        
        public Task<bool> DeleteAsync(int id, int recipientId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == id && n.RecipientId == recipientId);
            if (notification == null) return Task.FromResult(false);
            _notifications.Remove(notification);
            return Task.FromResult(true);
        }
        
        public void Reset()
        {
            _notifications.Clear();
            _nextId = 1;
        }
    }
}
