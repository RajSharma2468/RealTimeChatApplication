using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectHub.Presence.Services
{
    public class PresenceService : IPresenceService
    {
        private static readonly ConcurrentDictionary<int, string> _userConnections = new();
        private static readonly ConcurrentDictionary<int, DateTime> _lastSeen = new();
        
        public Task UserConnected(int userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
            _lastSeen[userId] = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        
        public Task UserDisconnected(int userId, string connectionId)
        {
            _userConnections.TryRemove(userId, out _);
            _lastSeen[userId] = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        
        public Task UpdateHeartbeat(int userId, string connectionId)
        {
            _lastSeen[userId] = DateTime.UtcNow;
            return Task.CompletedTask;
        }
        
        public Task<List<int>> GetOnlineUserIds()
        {
            return Task.FromResult(_userConnections.Keys.ToList());
        }
        
        public Task<DateTime?> GetLastSeen(int userId)
        {
            _lastSeen.TryGetValue(userId, out DateTime lastSeen);
            return Task.FromResult(lastSeen == default ? null : (DateTime?)lastSeen);
        }
        
        public Task<int> GetConnectionCount()
        {
            return Task.FromResult(_userConnections.Count);
        }
        
        public Task CleanupStaleConnections()
        {
            var staleTime = DateTime.UtcNow.AddMinutes(-5);
            var staleUsers = _lastSeen.Where(x => x.Value < staleTime).Select(x => x.Key).ToList();
            foreach (var userId in staleUsers)
            {
                _userConnections.TryRemove(userId, out _);
                _lastSeen.TryRemove(userId, out _);
            }
            return Task.CompletedTask;
        }
        
        public Task<bool> IsUserOnline(int userId)
        {
            return Task.FromResult(_userConnections.ContainsKey(userId));
        }
    }
}
