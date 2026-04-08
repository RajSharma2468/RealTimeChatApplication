using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectHub.Presence.Services;

namespace ConnectHub.Presence.Tests.Mocks
{
    public class MockPresenceService : IPresenceService
    {
        private readonly Dictionary<int, string> _userConnections = new();
        
        public Task UserConnected(int userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
            return Task.CompletedTask;
        }
        
        public Task UserDisconnected(int userId, string connectionId)
        {
            _userConnections.Remove(userId);
            return Task.CompletedTask;
        }
        
        public Task UpdateHeartbeat(int userId, string connectionId)
        {
            return Task.CompletedTask;
        }
        
        public Task<List<int>> GetOnlineUserIds()
        {
            return Task.FromResult(new List<int>(_userConnections.Keys));
        }
        
        public Task<DateTime?> GetLastSeen(int userId)
        {
            return Task.FromResult<DateTime?>(DateTime.UtcNow);
        }
        
        public Task<int> GetConnectionCount()
        {
            return Task.FromResult(_userConnections.Count);
        }
        
        public Task CleanupStaleConnections()
        {
            return Task.CompletedTask;
        }
        
        public Task<bool> IsUserOnline(int userId)
        {
            return Task.FromResult(_userConnections.ContainsKey(userId));
        }
    }
}