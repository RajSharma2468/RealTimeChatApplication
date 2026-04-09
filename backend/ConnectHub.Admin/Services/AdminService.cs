using ConnectHub.Admin.DTOs;
using ConnectHub.Admin.Models;
using ConnectHub.Admin.Repositories;

namespace ConnectHub.Admin.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        
        // ========== USER MANAGEMENT ==========
        
        public async Task<IEnumerable<UserAdminDto>> GetAllUsersAsync()
        {
            // In real implementation, call Auth service via HTTP
            // Return mock data for now
            var users = new List<UserAdminDto>
            {
                new UserAdminDto { Id = 1, Username = "admin", DisplayName = "Admin User", Email = "admin@example.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-30), MessageCount = 500 },
                new UserAdminDto { Id = 2, Username = "john_doe", DisplayName = "John Doe", Email = "john@example.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-20), MessageCount = 120 },
                new UserAdminDto { Id = 3, Username = "jane_smith", DisplayName = "Jane Smith", Email = "jane@example.com", IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-10), MessageCount = 45 }
            };
            
            return await Task.FromResult(users);
        }
        
        public async Task<UserAdminDto?> GetUserByIdAsync(int userId)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(u => u.Id == userId);
        }
        
        public async Task<bool> SuspendUserAsync(int adminId, int userId, string reason)
        {
            // In real implementation, call Auth service to set IsActive = false
            // Log the action
            var auditLog = new AuditLog
            {
                AdminId = adminId,
                Action = "SUSPEND_USER",
                TargetType = "USER",
                TargetId = userId,
                Details = $"{{\"reason\": \"{reason}\"}}",
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.CreateAuditLogAsync(auditLog);
            
            return await Task.FromResult(true);
        }
        
        public async Task<bool> DeleteUserAsync(int adminId, int userId)
        {
            var auditLog = new AuditLog
            {
                AdminId = adminId,
                Action = "DELETE_USER",
                TargetType = "USER",
                TargetId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.CreateAuditLogAsync(auditLog);
            
            return await Task.FromResult(true);
        }
        
        // ========== ROOM MANAGEMENT ==========
        
        public async Task<IEnumerable<RoomAdminDto>> GetAllRoomsAsync()
        {
            var rooms = new List<RoomAdminDto>
            {
                new RoomAdminDto { Id = 1, RoomName = "Cricket Fans", RoomType = "PUBLIC", CreatedBy = 1, CreatorName = "Admin", CreatedAt = DateTime.UtcNow.AddDays(-15), MemberCount = 45, MessageCount = 1200, IsActive = true },
                new RoomAdminDto { Id = 2, RoomName = "Football Fans", RoomType = "PUBLIC", CreatedBy = 2, CreatorName = "John Doe", CreatedAt = DateTime.UtcNow.AddDays(-10), MemberCount = 32, MessageCount = 850, IsActive = true },
                new RoomAdminDto { Id = 3, RoomName = "Spam Room", RoomType = "PUBLIC", CreatedBy = 5, CreatorName = "Spammer", CreatedAt = DateTime.UtcNow.AddDays(-2), MemberCount = 5, MessageCount = 200, IsActive = true }
            };
            
            return await Task.FromResult(rooms);
        }
        
        public async Task<bool> DeleteRoomAsync(int adminId, int roomId)
        {
            var auditLog = new AuditLog
            {
                AdminId = adminId,
                Action = "DELETE_ROOM",
                TargetType = "ROOM",
                TargetId = roomId,
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.CreateAuditLogAsync(auditLog);
            
            return await Task.FromResult(true);
        }
        
        // ========== MESSAGE MANAGEMENT ==========
        
        public async Task<IEnumerable<MessageAdminDto>> GetAllMessagesAsync()
        {
            var messages = new List<MessageAdminDto>
            {
                new MessageAdminDto { Id = 1, SenderId = 1, SenderName = "Admin", ReceiverId = 2, ReceiverName = "John Doe", Content = "Hello!", SentAt = DateTime.UtcNow.AddHours(-5), IsDeleted = false },
                new MessageAdminDto { Id = 2, SenderId = 2, SenderName = "John Doe", ReceiverId = 1, ReceiverName = "Admin", Content = "Hi there!", SentAt = DateTime.UtcNow.AddHours(-4), IsDeleted = false },
                new MessageAdminDto { Id = 3, SenderId = 5, SenderName = "Spammer", Content = "Spam message", SentAt = DateTime.UtcNow.AddHours(-1), IsDeleted = false }
            };
            
            return await Task.FromResult(messages);
        }
        
        public async Task<bool> DeleteMessageAsync(int adminId, int messageId)
        {
            var auditLog = new AuditLog
            {
                AdminId = adminId,
                Action = "DELETE_MESSAGE",
                TargetType = "MESSAGE",
                TargetId = messageId,
                CreatedAt = DateTime.UtcNow
            };
            await _adminRepository.CreateAuditLogAsync(auditLog);
            
            return await Task.FromResult(true);
        }
        
        // ========== ANALYTICS ==========
        
        public async Task<AnalyticsDto> GetAnalyticsAsync()
        {
            var analytics = new AnalyticsDto
            {
                TotalUsers = await _adminRepository.GetTotalUsersAsync(),
                ActiveUsers24h = await _adminRepository.GetActiveUsers24hAsync(),
                TotalMessages = await _adminRepository.GetTotalMessagesAsync(),
                MessagesToday = await _adminRepository.GetMessagesTodayAsync(),
                TotalRooms = await _adminRepository.GetTotalRoomsAsync(),
                ActiveConnections = await _adminRepository.GetActiveConnectionsAsync(),
                GeneratedAt = DateTime.UtcNow
            };
            
            return analytics;
        }
        
        // ========== AUDIT LOGS ==========
        
        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int page = 1, int pageSize = 50)
        {
            var logs = await _adminRepository.GetAuditLogsAsync(page, pageSize);
            
            return logs.Select(log => new AuditLogDto
            {
                Id = log.Id,
                AdminId = log.AdminId,
                AdminName = $"Admin_{log.AdminId}",
                Action = log.Action,
                TargetType = log.TargetType,
                TargetId = log.TargetId,
                Details = log.Details,
                IpAddress = log.IpAddress,
                CreatedAt = log.CreatedAt
            });
        }
    }
}