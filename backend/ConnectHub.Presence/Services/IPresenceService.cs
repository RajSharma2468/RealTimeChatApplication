using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectHub.Presence.Services
{
    public interface IPresenceService
    {
        Task UserConnected(int userId, string connectionId);
        Task UserDisconnected(int userId, string connectionId);
        Task UpdateHeartbeat(int userId, string connectionId);
        Task<List<int>> GetOnlineUserIds();
        Task<DateTime?> GetLastSeen(int userId);
        Task<int> GetConnectionCount();
        Task CleanupStaleConnections();
        Task<bool> IsUserOnline(int userId);
    }
}
