using ConnectHub.Presence.Services;
using System.Linq;

namespace ConnectHub.Presence.Tests.Fixtures
{
    [SetUpFixture]
    public class PresenceFixture
    {
        private static IPresenceService _presenceService;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _presenceService = new PresenceService();
            SeedTestData();
        }
        
        private void SeedTestData()
        {
            _presenceService.UserConnected(1, "fixture_conn_1");
            _presenceService.UserConnected(2, "fixture_conn_2");
            _presenceService.UserConnected(3, "fixture_conn_3");
        }
        
        public static IPresenceService GetPresenceService()
        {
            return _presenceService;
        }
        
        public static void ResetDatabase()
        {
            var onlineUsers = _presenceService.GetOnlineUserIds().Result.ToList();
            foreach (var userId in onlineUsers)
            {
                _presenceService.UserDisconnected(userId, $"conn_{userId}");
            }
            
            _presenceService.UserConnected(1, "fixture_conn_1");
            _presenceService.UserConnected(2, "fixture_conn_2");
            _presenceService.UserConnected(3, "fixture_conn_3");
        }
        
        [OneTimeTearDown]
        public void Teardown()
        {
            var onlineUsers = _presenceService.GetOnlineUserIds().Result.ToList();
            foreach (var userId in onlineUsers)
            {
                _presenceService.UserDisconnected(userId, $"conn_{userId}");
            }
        }
    }
}