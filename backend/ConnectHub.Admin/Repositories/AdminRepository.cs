using Microsoft.EntityFrameworkCore;
using ConnectHub.Admin.Models;
using ConnectHub.Admin.Data;

namespace ConnectHub.Admin.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AdminDbContext _context;
        
        public AdminRepository(AdminDbContext context)
        {
            _context = context;
        }
        
        // Create audit log entry
        public async Task<AuditLog> CreateAuditLogAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            return auditLog;
        }
        
        // Get all audit logs
        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int page = 1, int pageSize = 50)
        {
            int skip = (page - 1) * pageSize;
            
            return await _context.AuditLogs
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }
        
        // Get audit logs by specific admin
        public async Task<IEnumerable<AuditLog>> GetAuditLogsByAdminAsync(int adminId, int page = 1, int pageSize = 50)
        {
            int skip = (page - 1) * pageSize;
            
            return await _context.AuditLogs
                .Where(a => a.AdminId == adminId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }
        
        // Analytics - In real implementation, these would call other services
        // For now, return mock data
        public async Task<int> GetTotalUsersAsync()
        {
            // Would call Auth service via HTTP
            return await Task.FromResult(150);
        }
        
        public async Task<int> GetActiveUsers24hAsync()
        {
            return await Task.FromResult(45);
        }
        
        public async Task<int> GetTotalMessagesAsync()
        {
            return await Task.FromResult(12500);
        }
        
        public async Task<int> GetMessagesTodayAsync()
        {
            return await Task.FromResult(342);
        }
        
        public async Task<int> GetTotalRoomsAsync()
        {
            return await Task.FromResult(25);
        }
        
        public async Task<int> GetActiveConnectionsAsync()
        {
            return await Task.FromResult(38);
        }
    }
}