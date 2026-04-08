using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ConnectHub.Presence.Hubs;
using ConnectHub.Presence.Services;

namespace ConnectHub.Presence.Tests.UnitTests
{
    [TestFixture]
    public class PresenceHubTests
    {
        private Mock<IPresenceService> _mockPresenceService;
        private PresenceHub _presenceHub;
        private Mock<IHubCallerClients> _mockClients;
        private Mock<IClientProxy> _mockAllClients;

        [SetUp]
        public void Setup()
        {
            _mockPresenceService = new Mock<IPresenceService>();
            _presenceHub = new PresenceHub(_mockPresenceService.Object);
            
            // Setup Context
            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(x => x.UserIdentifier).Returns("1");
            mockContext.Setup(x => x.ConnectionId).Returns("test_conn_123");
            _presenceHub.Context = mockContext.Object;
            
            // Setup Clients with proper mocks
            _mockAllClients = new Mock<IClientProxy>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClients.Setup(x => x.All).Returns(_mockAllClients.Object);
            
            // Setup SendCoreAsync to return completed task
            _mockAllClients
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            _presenceHub.Clients = _mockClients.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _presenceHub?.Dispose();
            _presenceHub = null;
        }

        [Test]
        public async Task OnConnectedAsync_ValidUser_RegistersAndBroadcasts()
        {
            // Act
            await _presenceHub.OnConnectedAsync();
            
            // Assert - Service call verified
            _mockPresenceService.Verify(x => x.UserConnected(1, "test_conn_123"), Times.Once);
            
            // Assert - Broadcast verified
            _mockAllClients.Verify(x => x.SendCoreAsync(
                "UserOnline",
                It.Is<object[]>(o => o.Length == 1 && (int)o[0] == 1),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task OnDisconnectedAsync_ValidUser_UnregistersUser()
        {
            // Act
            await _presenceHub.OnDisconnectedAsync(null);
            
            // Assert
            _mockPresenceService.Verify(x => x.UserDisconnected(1, "test_conn_123"), Times.Once);
            _mockAllClients.Verify(x => x.SendCoreAsync(
                "UserOffline",
                It.Is<object[]>(o => o.Length == 1 && (int)o[0] == 1),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SendHeartbeat_UpdatesHeartbeat()
        {
            // Act
            await _presenceHub.SendHeartbeat();
            
            // Assert
            _mockPresenceService.Verify(x => x.UpdateHeartbeat(1, "test_conn_123"), Times.Once);
        }
    }
}