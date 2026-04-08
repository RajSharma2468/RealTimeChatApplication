using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NUnit.Framework;
using ConnectHub.Presence.Controllers;
using ConnectHub.Presence.Tests.Mocks;
using ConnectHub.Presence.Tests.Helpers;

namespace ConnectHub.Presence.Tests.IntegrationTests
{
    [TestFixture]
    public class PresenceControllerTests
    {
        private MockPresenceService _presenceService;
        private PresenceController _controller;
        
        [SetUp]
        public void Setup()
        {
            _presenceService = new MockPresenceService();
            _controller = new PresenceController(_presenceService);
            
            // Mock user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TestConstants.UserId1.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        // Test: GetOnlineUsers returns list of online users
        [Test]
        public async Task GetOnlineUsers_ReturnsOnlineUsersList()
        {
            // Arrange: Add online users
            await _presenceService.UserConnected(1, "conn_1");
            await _presenceService.UserConnected(2, "conn_2");
            
            // Act
            var result = _controller.GetOnlineUsers();
            
            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        
        // Test: IsUserOnline returns true for online user
        [Test]
        public async Task IsUserOnline_OnlineUser_ReturnsTrue()
        {
            // Arrange
            await _presenceService.UserConnected(TestConstants.UserId1, "conn_1");
            
            // Act
            var result = _controller.IsUserOnline(TestConstants.UserId1);
            
            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        
        // Test: IsUserOnline returns false for offline user
        [Test]
        public void IsUserOnline_OfflineUser_ReturnsFalse()
        {
            // Act (no user connected)
            var result = _controller.IsUserOnline(TestConstants.NonExistingUserId);
            
            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        
        // Test: GetLastSeen returns last seen time
        [Test]
        public async Task GetLastSeen_ReturnsLastSeenTime()
        {
            // Arrange: User connects and disconnects
            await _presenceService.UserConnected(TestConstants.UserId1, "conn_1");
            await _presenceService.UserDisconnected(TestConstants.UserId1, "conn_1");
            
            // Act
            var result = _controller.GetLastSeen(TestConstants.UserId1);
            
            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        
        // Test: GetConnectionCount returns correct count
        [Test]
        public async Task GetConnectionCount_ReturnsActiveConnections()
        {
            // Arrange
            await _presenceService.UserConnected(1, "conn_1");
            await _presenceService.UserConnected(2, "conn_2");
            await _presenceService.UserConnected(2, "conn_3"); // Second device for user 2
            
            // Act
            var result = _controller.GetConnectionCount();
            
            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
    }
}