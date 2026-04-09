using ConnectHub.Admin.Models;
using ConnectHub.Admin.Repositories;

namespace ConnectHub.Admin.Tests.Mocks
{
    public class MockAdminRepository : IAdminRepository
    {
        private readonly List<AuditLog> _auditLogs = new();
        private int _nextId = 1;
        
        // Audit log operations
        public Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog)
        {
            auditLog.Id = _nextId++;
            _auditLogs.Add(auditLog);
            return Task.FromResult(auditLog);
        }
        
        public Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page = 1, int pageSize = 50)
        {
            var result = _auditLogs
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(result.AsEnumerable());
        }
        
        public Task<IEnumerable<AuditLog>> GetAuditLogsByAdminAsync(int adminId, int page = 1, int pageSize = 50)
        {
            var result = _auditLogs
                .Where(a => a.AdminId == adminId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(result.AsEnumerable());
        }
        
        // Analytics methods
        public Task<int> GetTotalUsersAsync() => Task.FromResult(150);
        public Task<int> GetActiveUsers24hAsync() => Task.FromResult(45);
        public Task<int> GetTotalMessagesAsync() => Task.FromResult(12500);
        public Task<int> GetMessagesTodayAsync() => Task.FromResult(342);
        public Task<int> GetTotalRoomsAsync() => Task.FromResult(25);
        public Task<int> GetActiveConnectionsAsync() => Task.FromResult(38);
        
        // Helper method to reset state
        public void Reset()
        {
            _auditLogs.Clear();
            _nextId = 1;
        }
        
        // Helper to add test audit log
        public void AddTestAuditLog(AuditLog log)
        {
            log.Id = _nextId++;
            _auditLogs.Add(log);
        }
    }
}