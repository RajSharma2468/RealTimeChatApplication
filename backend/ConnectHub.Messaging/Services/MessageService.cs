using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Models;
using ConnectHub.Messaging.Repositories;

namespace ConnectHub.Messaging.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        
        // Send a new direct message
        public async Task<MessageResponseDto> SendDirectMessageAsync(int senderId, SendMessageDto dto)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                IsEdited = false,
                MessageType = "TEXT"
            };
            
            var created = await _messageRepository.CreateAsync(message);
            return MapToResponseDto(created);
        }
        
        // Get conversation history between two users
        public async Task<IEnumerable<MessageResponseDto>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize)
        {
            var messages = await _messageRepository.GetDirectMessagesAsync(userId1, userId2, page, pageSize);
            return messages.Select(MapToResponseDto);
        }
        
        public async Task<IEnumerable<MessageResponseDto>> GetRoomMessagesAsync(int roomId, int page, int pageSize)
        {
            var messages = await _messageRepository.GetRoomMessagesAsync(roomId, page, pageSize);
            return messages.Select(MapToResponseDto);
        }
        
        // Edit message - only sender can edit, only if not deleted
        public async Task<MessageResponseDto> EditMessageAsync(int userId, EditMessageDto dto)
        {
            var message = await _messageRepository.GetByIdAsync(dto.MessageId);
            
            if (message == null)
                throw new Exception("Message not found");
            
            if (message.SenderId != userId)
                throw new Exception("You can only edit your own messages");
            
            if (message.IsDeleted)
                throw new Exception("Cannot edit deleted message");
            
            message.Content = dto.NewContent;
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;
            
            var updated = await _messageRepository.UpdateAsync(message);
            return MapToResponseDto(updated);
        }
        
        // Soft delete message - only sender can delete
        public async Task<bool> DeleteMessageAsync(int userId, int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            
            if (message == null)
                throw new Exception("Message not found");
            
            if (message.SenderId != userId)
                throw new Exception("You can only delete your own messages");
            
            return await _messageRepository.SoftDeleteAsync(messageId);
        }
        
        // Search messages by keyword
        public async Task<IEnumerable<SearchMessageDto>> SearchMessagesAsync(int userId, string keyword, int? roomId = null)
        {
            var messages = await _messageRepository.SearchMessagesAsync(userId, keyword, roomId);
            
            return messages.Select(m => new SearchMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderName = $"User_{m.SenderId}",
                SentAt = m.SentAt,
                ConversationWith = m.RoomId.HasValue ? $"Room_{m.RoomId}" : $"User_{(m.SenderId == userId ? m.ReceiverId : m.SenderId).ToString()}"
            });
        }
        
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _messageRepository.GetUnreadCountAsync(userId);
        }
        
        public async Task<bool> MarkAsReadAsync(int userId, int messageId)
        {
            return await _messageRepository.MarkAsReadAsync(messageId, userId);
        }
        
        public async Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null)
        {
            return await _messageRepository.MarkAllAsReadAsync(userId, senderId);
        }
        
        public async Task<IEnumerable<MessageResponseDto>> GetRecentChatsAsync(int userId)
        {
            var messages = await _messageRepository.GetRecentChatsAsync(userId);
            return messages.Select(MapToResponseDto);
        }
        
        // Map Entity to DTO (hide sensitive data)
        private MessageResponseDto MapToResponseDto(Message message)
        {
            return new MessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = $"User_{message.SenderId}",
                ReceiverId = message.ReceiverId,
                ReceiverName = message.ReceiverId.HasValue ? $"User_{message.ReceiverId}" : null,
                RoomId = message.RoomId,
                Content = message.IsDeleted ? "[Message deleted]" : message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                IsDeleted = message.IsDeleted,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                MediaUrl = message.MediaUrl,
                MessageType = message.MessageType ?? "TEXT"
            };
        }
    }
}
