using System.Text.Json;
using ConnectHub.Notification.DTOs;
using ConnectHub.Notification.Models;
using ConnectHub.Notification.Repositories;

namespace ConnectHub.Notification.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<NotificationService> _logger;
        
        public NotificationService(
            INotificationRepository notificationRepository,
            IRabbitMqService rabbitMqService,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _rabbitMqService = rabbitMqService;
            _logger = logger;
        }
        
        // Send notification and publish to queue for offline processing
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
            
            // Publish to RabbitMQ for async processing (email, push notifications)
            var eventDto = new NotificationEventDto
            {
                EventType = dto.Type,
                RecipientId = dto.RecipientId,
                SenderId = dto.SenderId,
                Title = dto.Title,
                Message = dto.Message,
                RelatedId = dto.RelatedId,
                RelatedType = dto.RelatedType,
                OccurredAt = DateTime.UtcNow
            };
            
            await PublishNotificationEventAsync(eventDto);
            
            return MapToResponseDto(created);
        }
        
        // Publish event to RabbitMQ queue
        public async Task PublishNotificationEventAsync(NotificationEventDto eventDto)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(eventDto);
                _rabbitMqService.PublishMessage(jsonMessage, "notification");
                _logger.LogDebug("Notification event published for recipient {RecipientId}", eventDto.RecipientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish notification event to RabbitMQ");
            }
            
            await Task.CompletedTask;
        }
        
        // Process message from queue (called by consumer)
        public async Task ProcessQueueMessageAsync(string message)
        {
            try
            {
                var eventDto = JsonSerializer.Deserialize<NotificationEventDto>(message);
                if (eventDto == null) return;
                
                _logger.LogInformation("Processing notification event: {EventType} for user {RecipientId}", 
                    eventDto.EventType, eventDto.RecipientId);
                
                // Here you can:
                // 1. Send email if user is offline
                // 2. Send push notification
                // 3. Update analytics
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing queue message");
            }
        }
        
        // Rest of the methods remain the same...
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
            // Broadcast to all users (simplified)
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