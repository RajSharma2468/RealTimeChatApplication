using Microsoft.EntityFrameworkCore;
using ConnectHub.Messaging.Models;
using ConnectHub.Messaging.Data;

namespace ConnectHub.Messaging.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MessageDbContext _context;
        
        public MessageRepository(MessageDbContext context)
        {
            _context = context;
        }
        
        public async Task<Message> CreateAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
        
        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }
        
        // Get conversation between two users with pagination
        public async Task<IEnumerable<Message>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .Where(m => m.RoomId == null)
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(pageSize)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Message>> GetRoomMessagesAsync(int roomId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            
            return await _context.Messages
                .Where(m => m.RoomId == roomId)
                .OrderByDescending(m => m.SentAt)
                .Skip(skip)
                .Take(pageSize)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        
        public async Task<Message> UpdateAsync(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }
        
        // Soft delete - hide content but keep record
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message == null) return false;
            
            message.IsDeleted = true;
            message.Content = "[Message deleted]";
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Search messages by keyword
        public async Task<IEnumerable<Message>> SearchMessagesAsync(int userId, string keyword, int? roomId = null)
        {
            var query = _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) &&
                           m.Content.Contains(keyword));
            
            if (roomId.HasValue)
            {
                query = query.Where(m => m.RoomId == roomId);
            }
            
            return await query.OrderByDescending(m => m.SentAt).Take(50).ToListAsync();
        }
        
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }
        
        public async Task<bool> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == userId);
            
            if (message == null) return false;
            
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null)
        {
            var query = _context.Messages.Where(m => m.ReceiverId == userId && !m.IsRead);
            
            if (senderId.HasValue)
            {
                query = query.Where(m => m.SenderId == senderId);
            }
            
            var messages = await query.ToListAsync();
            
            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Get latest message from each conversation for sidebar
        public async Task<IEnumerable<Message>> GetRecentChatsAsync(int userId)
        {
            var directMessages = await _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.RoomId == null)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.SentAt).FirstOrDefault())
                .ToListAsync();
            
            return directMessages;
        }
    }
}
