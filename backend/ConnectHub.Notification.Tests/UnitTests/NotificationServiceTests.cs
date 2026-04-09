using Moq;
using NUnit.Framework;
using ConnectHub.Notification.Models;
using ConnectHub.Notification.DTOs;
using ConnectHub.Notification.Services;
using ConnectHub.Notification.Repositories;
using ConnectHub.Notification.Tests.Helpers;

namespace ConnectHub.Notification.Tests.UnitTests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<INotificationRepository> _mockRepo;
        private NotificationService _service;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<INotificationRepository>();
            _service = new NotificationService(_mockRepo.Object);
        }
        
        [Test]
        public async Task SendNotificationAsync_ValidData_CreatesNotification()
        {
            var dto = TestDataBuilder.CreateSendNotificationDto();
            var expected = new NotificationEntity { Id = 1, RecipientId = 2, Title = "New Message" };
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<NotificationEntity>())).ReturnsAsync(expected);
            var result = await _service.SendNotificationAsync(dto);
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public async Task GetMyNotificationsAsync_ReturnsUserNotifications()
        {
            var notifications = new List<NotificationEntity> { new NotificationEntity { Id = 1 } };
            _mockRepo.Setup(x => x.GetByRecipientAsync(1, 1, 20)).ReturnsAsync(notifications);
            var result = await _service.GetMyNotificationsAsync(1);
            Assert.That(result.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public async Task GetUnreadCountAsync_ReturnsCount()
        {
            _mockRepo.Setup(x => x.GetUnreadCountAsync(1)).ReturnsAsync(5);
            var count = await _service.GetUnreadCountAsync(1);
            Assert.That(count, Is.EqualTo(5));
        }
        
        [Test]
        public async Task MarkAsReadAsync_ValidId_ReturnsTrue()
        {
            _mockRepo.Setup(x => x.MarkAsReadAsync(1, 1)).ReturnsAsync(true);
            var result = await _service.MarkAsReadAsync(1, 1);
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task MarkAllAsReadAsync_ReturnsTrue()
        {
            _mockRepo.Setup(x => x.MarkAllAsReadAsync(1)).ReturnsAsync(true);
            var result = await _service.MarkAllAsReadAsync(1);
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteNotificationAsync_ValidId_ReturnsTrue()
        {
            _mockRepo.Setup(x => x.DeleteAsync(1, 1)).ReturnsAsync(true);
            var result = await _service.DeleteNotificationAsync(1, 1);
            Assert.That(result, Is.True);
        }
    }
}
