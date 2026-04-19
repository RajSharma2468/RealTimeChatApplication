using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Models;
using ConnectHub.Messaging.Repositories;
using System.Text.Json;

namespace ConnectHub.Messaging.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public MessageService(IMessageRepository messageRepository, IHttpClientFactory httpClientFactory)
        {
            _messageRepository = messageRepository;
            _httpClientFactory = httpClientFactory;
        }
        
        // ================================================================
        // HELPER: Get real user name from Auth Service
        // Token passed for server-to-server authenticated call
        // ================================================================
        private async Task<string> GetUserNameFromAuth(int userId, string token = null)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                
                var response = await httpClient.GetAsync($"https://connecthub-auth-brdsdmghhhgwaphq.centralus-01.azurewebsites.net/api/Auth/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var userData = JsonSerializer.Deserialize<AuthUserResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (userData?.Success == true && userData.Data != null)
                    {
                        return userData.Data.DisplayName ?? userData.Data.Username ?? $"User_{userId}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch user {userId}: {ex.Message}");
            }
            return $"User_{userId}";
        }
        
        // ================================================================
        // SEND DIRECT MESSAGE
        // ================================================================
        public async Task<MessageResponseDto> SendDirectMessageAsync(int senderId, SendMessageDto dto, string senderName = null)
        {
            if (string.IsNullOrEmpty(senderName))
            {
                senderName = await GetUserNameFromAuth(senderId);
            }
            
            var message = new Message
            {
                SenderId = senderId,
                SenderName = senderName,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                IsDeletedForSender = false,
                IsDeletedForReceiver = false,
                IsEdited = false,
                MessageType = "TEXT"
            };
            
            var created = await _messageRepository.CreateAsync(message);
            return MapToResponseDto(created);
        }
        
        // ================================================================
        // SEND ROOM MESSAGE
        // ================================================================
        public async Task<MessageResponseDto> SendRoomMessageAsync(int senderId, SendRoomMessageDto dto, string senderName = null)
        {
            if (string.IsNullOrEmpty(senderName))
            {
                senderName = await GetUserNameFromAuth(senderId);
            }
            
            var message = new Message
            {
                SenderId = senderId,
                SenderName = senderName,
                RoomId = dto.RoomId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsDeleted = false,
                IsDeletedForSender = false,
                IsDeletedForReceiver = false,
                IsEdited = false,
                MessageType = "TEXT"
            };
            
            var created = await _messageRepository.CreateAsync(message);
            return MapToResponseDto(created);
        }
        
        // ================================================================
        // GET DIRECT MESSAGES
        // Token passed from controller for Auth Service calls
        // ================================================================
        public async Task<IEnumerable<MessageResponseDto>> GetDirectMessagesAsync(int userId1, int userId2, int page, int pageSize, string token = null)
        {
            var messages = await _messageRepository.GetDirectMessagesAsync(userId1, userId2, page, pageSize);
            
            var filteredMessages = messages.Where(m => 
                !(m.IsDeletedForSender && m.SenderId == userId1) &&
                !(m.IsDeletedForReceiver && m.ReceiverId == userId1) &&
                !(m.IsDeletedForSender && m.SenderId == userId2) &&
                !(m.IsDeletedForReceiver && m.ReceiverId == userId2)
            ).ToList();

            var userNameCache = new Dictionary<int, string>();
            var result = new List<MessageResponseDto>();
            
            foreach (var m in filteredMessages)
            {
                if (!userNameCache.ContainsKey(m.SenderId))
                {
                    userNameCache[m.SenderId] = await GetUserNameFromAuth(m.SenderId, token);
                }
                m.SenderName = userNameCache[m.SenderId];
                result.Add(MapToResponseDto(m));
            }

            return result;
        }
        
        // ================================================================
        // GET ROOM MESSAGES
        // Token passed from controller for Auth Service calls
        // ================================================================
        public async Task<IEnumerable<MessageResponseDto>> GetRoomMessagesAsync(int roomId, int page, int pageSize, string token = null)
        {
            var messages = await _messageRepository.GetRoomMessagesAsync(roomId, page, pageSize);
            var filteredMessages = messages.Where(m => !m.IsDeleted).ToList();

            var userNameCache = new Dictionary<int, string>();
            var result = new List<MessageResponseDto>();
            
            foreach (var m in filteredMessages)
            {
                if (!userNameCache.ContainsKey(m.SenderId))
                {
                    userNameCache[m.SenderId] = await GetUserNameFromAuth(m.SenderId, token);
                }
                m.SenderName = userNameCache[m.SenderId];
                result.Add(MapToResponseDto(m));
            }

            return result;
        }
        
        // ================================================================
        // EDIT MESSAGE
        // ================================================================
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
        
        // ================================================================
        // DELETE MESSAGE
        // ================================================================
        public async Task<bool> DeleteMessageAsync(int userId, DeleteMessageDto dto)
        {
            var message = await _messageRepository.GetByIdAsync(dto.MessageId);
            
            if (message == null)
                throw new Exception("Message not found");
            
            bool isSender = (message.SenderId == userId);
            bool isReceiver = (message.ReceiverId == userId);
            
            if (dto.DeleteType == "FOR_EVERYONE")
            {
                if (!isSender)
                    throw new Exception("Only the sender can delete this message for everyone");
                
                message.IsDeleted = true;
                message.Content = "[Message deleted]";
                await _messageRepository.UpdateAsync(message);
            }
            else
            {
                if (isSender)
                {
                    message.IsDeletedForSender = true;
                }
                else if (isReceiver)
                {
                    message.IsDeletedForReceiver = true;
                }
                else
                {
                    throw new Exception("You are not a participant of this message");
                }
                
                await _messageRepository.UpdateAsync(message);
            }
            
            return true;
        }
        
        // ================================================================
        // SEARCH MESSAGES
        // ================================================================
        public async Task<IEnumerable<SearchMessageDto>> SearchMessagesAsync(int userId, string keyword, int? roomId = null)
        {
            var messages = await _messageRepository.SearchMessagesAsync(userId, keyword, roomId);
            
            return messages.Select(m => new SearchMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderName = m.SenderName ?? $"User_{m.SenderId}",
                SentAt = m.SentAt,
                ConversationWith = m.RoomId.HasValue ? $"Room_{m.RoomId}" : $"User_{(m.SenderId == userId ? m.ReceiverId : m.SenderId).ToString()}"
            });
        }
        
        // ================================================================
        // GET UNREAD COUNT
        // ================================================================
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _messageRepository.GetUnreadCountAsync(userId);
        }
        
        // ================================================================
        // MARK AS READ
        // ================================================================
        public async Task<bool> MarkAsReadAsync(int userId, int messageId)
        {
            return await _messageRepository.MarkAsReadAsync(messageId, userId);
        }
        
        // ================================================================
        // MARK ALL AS READ
        // ================================================================
        public async Task<bool> MarkAllAsReadAsync(int userId, int? senderId = null)
        {
            return await _messageRepository.MarkAllAsReadAsync(userId, senderId);
        }
        
        // ================================================================
        // GET RECENT CHATS
        // ================================================================
        public async Task<IEnumerable<RecentChatDto>> GetRecentChatsAsync(int userId, string token = null)
        {
            var messages = await _messageRepository.GetRecentChatsAsync(userId);
            var result = new List<RecentChatDto>();
            
            using var httpClient = _httpClientFactory.CreateClient();
            
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
            
            foreach (var msg in messages)
            {
                int otherUserId = msg.SenderId == userId ? (msg.ReceiverId ?? 0) : msg.SenderId;
                if (otherUserId == 0) continue;
                
                string username = $"user_{otherUserId}";
                string displayName = $"User {otherUserId}";
                string? avatarUrl = null;
                
                try
                {
                    var response = await httpClient.GetAsync($"https://connecthub-auth-brdsdmghhhgwaphq.centralus-01.azurewebsites.net/api/Auth/{otherUserId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var userData = JsonSerializer.Deserialize<AuthUserResponse>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (userData?.Success == true && userData.Data != null)
                        {
                            username = userData.Data.Username ?? $"user_{otherUserId}";
                            displayName = userData.Data.DisplayName ?? userData.Data.Username ?? $"User {otherUserId}";
                            avatarUrl = userData.Data.AvatarUrl;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch user {otherUserId}: {ex.Message}");
                }
                
                int unreadCount = await _messageRepository.GetUnreadCountFromUserAsync(userId, otherUserId);
                
                result.Add(new RecentChatDto
                {
                    UserId = otherUserId,
                    Username = username,
                    DisplayName = displayName,
                    AvatarUrl = avatarUrl,
                    LastMessage = msg.IsDeleted ? "[Message deleted]" : (msg.Content?.Length > 50 ? msg.Content.Substring(0, 50) + "..." : msg.Content ?? ""),
                    LastMessageTime = msg.SentAt,
                    UnreadCount = unreadCount
                });
            }
            
            return result.OrderByDescending(x => x.LastMessageTime);
        }
        
        // ================================================================
        // MAP ENTITY TO DTO
        // ================================================================
        private MessageResponseDto MapToResponseDto(Message message)
        {
            return new MessageResponseDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = message.SenderName ?? $"User_{message.SenderId}",
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
    
    public class AuthUserResponse
    {
        public bool Success { get; set; }
        public AuthUserData Data { get; set; }
    }
    
    public class AuthUserData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string? AvatarUrl { get; set; }
    }
}