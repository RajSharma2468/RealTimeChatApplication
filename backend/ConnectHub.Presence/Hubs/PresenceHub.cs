using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using ConnectHub.Presence.Services;

namespace ConnectHub.Presence.Hubs
{
    public class PresenceHub : Hub
    {
        private readonly IPresenceService _presenceService;
        
        public PresenceHub(IPresenceService presenceService)
        {
            _presenceService = presenceService;
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
    }
}
