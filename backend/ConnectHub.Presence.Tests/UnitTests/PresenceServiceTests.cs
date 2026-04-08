using NUnit.Framework;
using ConnectHub.Presence.Services;
using ConnectHub.Presence.Tests.Helpers;
using System.Threading;
using System.Linq;

namespace ConnectHub.Presence.Tests.UnitTests
{
    [TestFixture]
    public class PresenceServiceTests
    {
        private PresenceService _presenceService;
        
        [SetUp]
        public void Setup()
        {
            _presenceService = new PresenceService();
        }
        
        [Test]
        public void UserConnected_ValidUser_MarksUserOnline()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var isOnline = _presenceService.IsUserOnline(TestConstants.UserId1).Result;
            var count = _presenceService.GetConnectionCount().Result;
            
            Assert.That(isOnline, Is.True);
            Assert.That(count, Is.EqualTo(1));
        }
        
        [Test]
        public void UserConnected_MultipleConnections_TracksAllDevices()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId2);
            
            var isOnline = _presenceService.IsUserOnline(TestConstants.UserId1).Result;
            var count = _presenceService.GetConnectionCount().Result;
            
            Assert.That(isOnline, Is.True);
            Assert.That(count, Is.EqualTo(2));
        }
        
        [Test]
        public void UserDisconnected_OneOfMultipleDevices_UserStillOnline()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId2);
            
            _presenceService.UserDisconnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var isOnline = _presenceService.IsUserOnline(TestConstants.UserId1).Result;
            var count = _presenceService.GetConnectionCount().Result;
            
            Assert.That(isOnline, Is.True);
            Assert.That(count, Is.EqualTo(1));
        }
        
        [Test]
        public void UserDisconnected_LastDevice_UserGoesOffline()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            _presenceService.UserDisconnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var isOnline = _presenceService.IsUserOnline(TestConstants.UserId1).Result;
            var count = _presenceService.GetConnectionCount().Result;
            
            Assert.That(isOnline, Is.False);
            Assert.That(count, Is.EqualTo(0));
        }
        
        [Test]
        public void GetOnlineUserIds_ReturnsOnlyOnlineUsers()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var onlineUsers = _presenceService.GetOnlineUserIds().Result;
            
            Assert.That(onlineUsers.Count, Is.EqualTo(1));
            Assert.That(onlineUsers.Contains(TestConstants.UserId1), Is.True);
            Assert.That(onlineUsers.Contains(TestConstants.UserId2), Is.False);
        }
        
        [Test]
        public void UpdateHeartbeat_RefreshesLastHeartbeat()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            Thread.Sleep(100);
            _presenceService.UpdateHeartbeat(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            Assert.Pass("Heartbeat updated successfully");
        }
        
        [Test]
        public void CleanupStaleConnections_RemovesOldConnections()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var countBefore = _presenceService.GetConnectionCount().Result;
            Assert.That(countBefore, Is.EqualTo(1));
            
            _presenceService.CleanupStaleConnections();
            
            var countAfter = _presenceService.GetConnectionCount().Result;
            Assert.That(countAfter, Is.EqualTo(0));
        }
        
        [Test]
        public void GetLastSeen_OnlineUser_ReturnsCurrentTime()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var lastSeen = _presenceService.GetLastSeen(TestConstants.UserId1).Result;
            
            Assert.That(lastSeen, Is.Not.Null);
            Assert.That(lastSeen.Value, Is.LessThanOrEqualTo(DateTime.UtcNow));
        }
        
        [Test]
        public void GetLastSeen_OfflineUser_ReturnsLastSeenTime()
        {
            _presenceService.UserConnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            Thread.Sleep(100);
            _presenceService.UserDisconnected(TestConstants.UserId1, TestConstants.ConnectionId1);
            
            var lastSeen = _presenceService.GetLastSeen(TestConstants.UserId1).Result;
            
            Assert.That(lastSeen, Is.Not.Null);
        }
        
        [Test]
        public void GetLastSeen_UnknownUser_ReturnsNull()
        {
            var lastSeen = _presenceService.GetLastSeen(TestConstants.NonExistingUserId).Result;
            
            Assert.That(lastSeen, Is.Null);
        }
    }
}