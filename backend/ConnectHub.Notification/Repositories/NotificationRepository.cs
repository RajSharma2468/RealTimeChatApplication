using Microsoft.EntityFrameworkCore;
using ConnectHub.Notification.Models;
using ConnectHub.Notification.Data;

namespace ConnectHub.Notification.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;
        
        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }
        
        public async Task<NotificationEntity> CreateAsync(NotificationEntity notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        
        public async Task<NotificationEntity?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }
        
        public async Task<IEnumerable<NotificationEntity>> GetByRecipientAsync(int recipientId, int page = 1, int pageSize = 20)
        {
            int skip = (page - 1) * pageSize;
            
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.SentAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<NotificationEntity>> GetUnreadByRecipientAsync(int recipientId)
        {
            return await _context.Notifications
                .Where(n => n.RecipientId == recipientId && !n.IsRead)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }
        
        public async Task<int> GetUnreadCountAsync(int recipientId)
        {
            return await _context.Notifications
                .CountAsync(n => n.RecipientId == recipientId && !n.IsRead);
        }
        
        public async Task<bool> MarkAsReadAsync(int id, int recipientId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.RecipientId == recipientId);
            
            if (notification == null) return false;
            
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> MarkAllAsReadAsync(int recipientId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == recipientId && !n.IsRead)
                .ToListAsync();
            
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> DeleteAsync(int id, int recipientId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.RecipientId == recipientId);
            
            if (notification == null) return false;
            
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
