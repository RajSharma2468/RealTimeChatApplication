using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using ConnectHub.Presence.Services;
using ConnectHub.Presence.DTOs;

namespace ConnectHub.Presence.Hubs
{
    public class PresenceHub : Hub
    {
        private readonly IPresenceService _presenceService;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public PresenceHub(IPresenceService presenceService, IHttpClientFactory httpClientFactory)
        {
            _presenceService = presenceService;
            _httpClientFactory = httpClientFactory;
        }
        
        private int GetUserId()
        {
            var userIdString = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(userIdString))
                throw new InvalidOperationException("User identifier is missing");
            if (!int.TryParse(userIdString, out int userId))
                throw new InvalidOperationException("Invalid user identifier format");
            return userId;
        }
        
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;
            await _presenceService.UserConnected(userId, connectionId);
            
            if (Clients?.All != null)
                await Clients.All.SendAsync("UserOnline", userId);
            
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;
            await _presenceService.UserDisconnected(userId, connectionId);
            
            if (Clients?.All != null)
                await Clients.All.SendAsync("UserOffline", userId);
            
            await base.OnDisconnectedAsync(exception);
        }
        
        public async Task SendHeartbeat()
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;
            await _presenceService.UpdateHeartbeat(userId, connectionId);
        }
        
        // ================================================================
        // TYPING INDICATOR
        // ================================================================
        public async Task SendTypingIndicator(TypingIndicatorDto dto)
        {
            var userId = GetUserId();
            dto.SenderId = userId;
            
            if (dto.ConversationType == "DIRECT")
            {
                await Clients.User(dto.ReceiverId.ToString())
                    .SendAsync("UserTyping", dto);
            }
            else if (dto.ConversationType == "ROOM" && dto.RoomId.HasValue)
            {
                await Clients.Group(dto.RoomId.ToString())
                    .SendAsync("UserTyping", dto);
            }
        }
        
        // ================================================================
        // SEND DIRECT MESSAGE
        // ================================================================
        public async Task SendDirectMessage(int receiverId, string content, string senderName)
        {
            var senderId = GetUserId();
            
            var message = new
            {
                Id = 0,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                SenderName = string.IsNullOrEmpty(senderName) ? $"User_{senderId}" : senderName
            };
            
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
        
        // ================================================================
        // SEND ROOM MESSAGE
        // ================================================================
        public async Task SendRoomMessage(int roomId, string content, string senderName)
        {
            var senderId = GetUserId();
            
            var message = new
            {
                Id = 0,
                SenderId = senderId,
                RoomId = roomId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                SenderName = string.IsNullOrEmpty(senderName) ? $"User_{senderId}" : senderName
            };
            
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveRoomMessage", message);
        }
        
        // ================================================================
        // MARK MESSAGE AS READ
        // ================================================================
        public async Task MarkMessageAsRead(int messageId, int senderId)
        {
            var readerId = GetUserId();
            
            try
            {
                var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("No token found in connection");
                    return;
                }
                
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                
                var response = await httpClient.PutAsync(
                    $"http://localhost:5289/api/message/read/{messageId}",
                    null);
                
                if (response.IsSuccessStatusCode)
                {
                    await Clients.User(senderId.ToString())
                        .SendAsync("MessageRead", new { messageId, readerId });
                    Console.WriteLine($"Message {messageId} marked as read by user {readerId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to mark message as read: {ex.Message}");
            }
        }
        
        // ================================================================
        // JOIN ROOM GROUP
        // ================================================================
        public async Task JoinRoomGroup(int roomId)
        {
            var userId = GetUserId();
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            Console.WriteLine($"User {userId} joined SignalR group for room {roomId}");
        }
        
        // ================================================================
        // LEAVE ROOM GROUP
        // ================================================================
        public async Task LeaveRoomGroup(int roomId)
        {
            var userId = GetUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
            Console.WriteLine($"User {userId} left SignalR group for room {roomId}");
        }
    }
}