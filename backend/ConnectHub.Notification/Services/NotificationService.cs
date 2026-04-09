using ConnectHub.Notification.DTOs;
using ConnectHub.Notification.Models;
using ConnectHub.Notification.Repositories;

namespace ConnectHub.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        
        public async Task<NotificationResponseDto> SendNotificationAsync(SendNotificationDto dto)
        {
            var notification = new NotificationEntity
            {
                RecipientId = dto.RecipientId,
                SenderId = dto.SenderId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RelatedId = dto.RelatedId,
                RelatedType = dto.RelatedType,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            
            var created = await _notificationRepository.CreateAsync(notification);
            return MapToResponseDto(created);
        }
        
        public async Task<IEnumerable<NotificationResponseDto>> GetMyNotificationsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var notifications = await _notificationRepository.GetByRecipientAsync(userId, page, pageSize);
            return notifications.Select(MapToResponseDto);
        }
        
        public async Task<IEnumerable<NotificationResponseDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByRecipientAsync(userId);
            return notifications.Select(MapToResponseDto);
        }
        
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }
        
        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        }
        
        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }
        
        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            return await _notificationRepository.DeleteAsync(notificationId, userId);
        }
        
        public async Task<bool> BroadcastNotificationAsync(int adminUserId, BroadcastNotificationDto dto)
        {
            return await Task.FromResult(true);
        }
        
        private NotificationResponseDto MapToResponseDto(NotificationEntity notification)
        {
            return new NotificationResponseDto
            {
                Id = notification.Id,
                RecipientId = notification.RecipientId,
                SenderId = notification.SenderId,
                SenderName = notification.SenderId.HasValue ? $"User_{notification.SenderId}" : null,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                RelatedId = notification.RelatedId,
                RelatedType = notification.RelatedType,
                IsRead = notification.IsRead,
                SentAt = notification.SentAt
            };
        }
    }
}
