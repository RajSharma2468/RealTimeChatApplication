using ConnectHub.Admin.DTOs;

namespace ConnectHub.Admin.Services
{
    public interface IAdminService
    {
        // User management
        Task<IEnumerable<UserAdminDto>> GetAllUsersAsync();
        Task<UserAdminDto?> GetUserByIdAsync(int userId);
        Task<bool> SuspendUserAsync(int adminId, int userId, string reason);
        Task<bool> DeleteUserAsync(int adminId, int userId);
        
        // Room management
        Task<IEnumerable<RoomAdminDto>> GetAllRoomsAsync();
        Task<bool> DeleteRoomAsync(int adminId, int roomId);
        
        // Message management
        Task<IEnumerable<MessageAdminDto>> GetAllMessagesAsync();
        Task<bool> DeleteMessageAsync(int adminId, int messageId);
        
        // Analytics
        Task<AnalyticsDto> GetAnalyticsAsync();
        
        // Audit logs
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(int page = 1, int pageSize = 50);
    }
}