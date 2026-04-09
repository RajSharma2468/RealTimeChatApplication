using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using ConnectHub.Notification.Controllers;
using ConnectHub.Notification.Services;
using ConnectHub.Notification.DTOs;
using ConnectHub.Notification.Tests.Helpers;

namespace ConnectHub.Notification.Tests.IntegrationTests
{
    [TestFixture]
    public class NotificationControllerTests
    {
        private Mock<INotificationService> _mockService;
        private NotificationController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<INotificationService>();
            _controller = new NotificationController(_mockService.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        [Test]
        public async Task GetMyNotifications_ReturnsOk()
        {
            var result = await _controller.GetMyNotifications(1, 20);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetUnreadCount_ReturnsOk()
        {
            var result = await _controller.GetUnreadCount();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetUnreadNotifications_ReturnsOk()
        {
            var result = await _controller.GetUnreadNotifications();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task MarkAsRead_ValidId_ReturnsOk()
        {
            _mockService.Setup(x => x.MarkAsReadAsync(1, 1)).ReturnsAsync(true);
            var result = await _controller.MarkAsRead(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task MarkAsRead_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(x => x.MarkAsReadAsync(999, 1)).ReturnsAsync(false);
            var result = await _controller.MarkAsRead(999);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        
        [Test]
        public async Task MarkAllAsRead_ReturnsOk()
        {
            var result = await _controller.MarkAllAsRead();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteNotification_ValidId_ReturnsOk()
        {
            _mockService.Setup(x => x.DeleteNotificationAsync(1, 1)).ReturnsAsync(true);
            var result = await _controller.DeleteNotification(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteNotification_InvalidId_ReturnsNotFound()
        {
            _mockService.Setup(x => x.DeleteNotificationAsync(999, 1)).ReturnsAsync(false);
            var result = await _controller.DeleteNotification(999);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        
        [Test]
        public async Task Broadcast_ReturnsOk()
        {
            var dto = new BroadcastNotificationDto { Title = "Test", Message = "Test" };
            var result = await _controller.Broadcast(dto);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}
