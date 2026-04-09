using ConnectHub.Admin.Models;

namespace ConnectHub.Admin.Repositories
{
    public interface IAdminRepository
    {
        // Audit log operations
        Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<AuditLog>> GetAuditLogsByAdminAsync(int adminId, int page = 1, int pageSize = 50);
        
        // Analytics (calls other services or uses cached data)
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveUsers24hAsync();
        Task<int> GetTotalMessagesAsync();
        Task<int> GetMessagesTodayAsync();
        Task<int> GetTotalRoomsAsync();
        Task<int> GetActiveConnectionsAsync();
    }
}