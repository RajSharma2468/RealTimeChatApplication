using ConnectHub.Messaging.Models;

namespace ConnectHub.Messaging.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> CreateAsync(Message message);
        Task<Message?> GetByIdAsync(int id);
        Task<IEnumerable<Message>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize);
        Task<IEnumerable<Message>> GetRoomMessagesAsync(int roomId, int page, int pageSize);
        Task<Message> UpdateAsync(Message message);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<Message>> SearchMessagesAsync(int userId, string keyword, int? roomId = null);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int messageId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null);
        Task<IEnumerable<Message>> GetRecentChatsAsync(int userId);
    }
}
