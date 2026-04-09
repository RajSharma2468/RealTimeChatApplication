using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ConnectHub.Notification.Services;

namespace ConnectHub.Notification.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        
        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        private int GetUserId()
        {
            var userIdClaim = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = Context.User?.FindFirst("nameid")?.Value;
            }
            return int.Parse(userIdClaim);
        }
        
        public async Task SendUnreadCount()
        {
            var userId = GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            await Clients.Caller.SendAsync("NotificationCount", count);
        }
    }
}
