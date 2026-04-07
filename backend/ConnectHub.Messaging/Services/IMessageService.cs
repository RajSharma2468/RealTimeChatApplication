using ConnectHub.Messaging.DTOs;

namespace ConnectHub.Messaging.Services
{
    public interface IMessageService
    {
        Task<MessageResponseDto> SendDirectMessageAsync(int senderId, SendMessageDto dto);
        Task<IEnumerable<MessageResponseDto>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize);
        Task<IEnumerable<MessageResponseDto>> GetRoomMessagesAsync(int roomId, int page, int pageSize);
        Task<MessageResponseDto> EditMessageAsync(int userId, EditMessageDto dto);
        Task<bool> DeleteMessageAsync(int userId, int messageId);
        Task<IEnumerable<SearchMessageDto>> SearchMessagesAsync(int userId, string keyword, int? roomId = null);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int messageId);
        Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null);
        Task<IEnumerable<MessageResponseDto>> GetRecentChatsAsync(int userId);
    }
}
