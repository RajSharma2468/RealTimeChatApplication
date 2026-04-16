using ConnectHub.Messaging.DTOs;

namespace ConnectHub.Messaging.Services
{
    public interface IMessageService
    {
        // Send messages
        Task<MessageResponseDto> SendDirectMessageAsync(int senderId, SendMessageDto dto, string senderName = null);
        Task<MessageResponseDto> SendRoomMessageAsync(int senderId, SendRoomMessageDto dto, string senderName = null);
        
        // Get messages
        Task<IEnumerable<MessageResponseDto>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize);
        Task<IEnumerable<MessageResponseDto>> GetRoomMessagesAsync(int roomId, int page, int pageSize);
        Task<IEnumerable<SearchMessageDto>> SearchMessagesAsync(int userId, string keyword, int? roomId = null);
        
        // Message actions
        Task<MessageResponseDto> EditMessageAsync(int userId, EditMessageDto dto);
        Task<bool> DeleteMessageAsync(int userId, DeleteMessageDto dto);
        
        // Read receipts
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int messageId);
        Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null);
        
        // Recent chats - ADD token parameter
        Task<IEnumerable<RecentChatDto>> GetRecentChatsAsync(int userId, string token = null);
    }
}