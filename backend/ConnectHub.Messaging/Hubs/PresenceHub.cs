using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ConnectHub.Messaging.Hubs
{
    // DTO for typing indicator
    public class TypingIndicatorDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public bool IsTyping { get; set; }
        public string ConversationType { get; set; } = "DIRECT";
        public int? RoomId { get; set; }
    }

    public class PresenceHub : Hub
    {
        private static Dictionary<int, string> _userConnections = new();
        
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _userConnections[userId.Value] = Context.ConnectionId;
                await Clients.All.SendAsync("UserOnline", userId.Value);
                Console.WriteLine($"User {userId.Value} connected");
            }
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _userConnections.Remove(userId.Value);
                await Clients.All.SendAsync("UserOffline", userId.Value);
                Console.WriteLine($"User {userId.Value} disconnected");
            }
            await base.OnDisconnectedAsync(exception);
        }
        
        private int? GetUserId()
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
                return userId;
            return null;
        }
        
        public async Task SendHeartbeat()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                Console.WriteLine($"Heartbeat received from user {userId.Value}");
            }
            await Task.CompletedTask;
        }
        
        public async Task SendTypingIndicator(TypingIndicatorDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;
            
            dto.SenderId = userId.Value;
            
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
        // SEND DIRECT MESSAGE - WITH DATABASE SAVE
        // ================================================================
        public async Task SendDirectMessage(int receiverId, string content, string senderName)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue) return;
            
            // Get token from query string
            var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
            int savedMessageId = 0;
            
            // ================================================================
            // SAVE TO DATABASE FIRST
            // ================================================================
            try
            {
                using var httpClient = new HttpClient();
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                
                var saveData = new { receiverId, content, senderName };
                var json = JsonSerializer.Serialize(saveData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync("http://localhost:5289/api/message/send", httpContent);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SaveMessageResponse>(responseJson);
                    if (result?.Success == true && result.Data?.Id > 0)
                    {
                        savedMessageId = result.Data.Id;
                        Console.WriteLine($"Message saved to database with ID: {savedMessageId}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to save message: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception saving message: {ex.Message}");
            }
            
            // ================================================================
            // SEND REAL-TIME MESSAGE WITH DATABASE ID
            // ================================================================
            var message = new
            {
                Id = savedMessageId,
                SenderId = senderId.Value,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                SenderName = senderName ?? $"User_{senderId}"
            };
            
            if (_userConnections.ContainsKey(receiverId))
            {
                await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", message);
            }
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
        
        public async Task JoinRoomGroup(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }
        
        public async Task LeaveRoomGroup(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }
        
        public async Task MarkMessageAsRead(int messageId, int senderId)
        {
            var readerId = GetUserId();
            if (!readerId.HasValue) return;
            
            try
            {
                var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("No token found in connection");
                    return;
                }
                
                using var httpClient = new HttpClient();
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
    }
    
    // ================================================================
    // RESPONSE CLASSES FOR DATABASE SAVE
    // ================================================================
    public class SaveMessageResponse
    {
        public bool Success { get; set; }
        public MessageData Data { get; set; }
    }
    
    public class MessageData
    {
        public int Id { get; set; }
    }
}