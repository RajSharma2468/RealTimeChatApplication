using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ConnectHub.Notification.Data;
using ConnectHub.Notification.Models;
using ConnectHub.Notification.Repositories;
using ConnectHub.Notification.Tests.Helpers;

namespace ConnectHub.Notification.Tests.UnitTests
{
    [TestFixture]
    public class NotificationRepositoryTests
    {
        private NotificationDbContext _context;
        private NotificationRepository _repository;
        
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<NotificationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new NotificationDbContext(options);
            _repository = new NotificationRepository(_context);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
        
        [Test]
        public async Task CreateAsync_ValidNotification_AddsToDatabase()
        {
            var notification = TestDataBuilder.CreateTestNotification();
            var result = await _repository.CreateAsync(notification);
            Assert.That(result.Id, Is.GreaterThan(0));
        }
        
        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsNotification()
        {
            var notification = TestDataBuilder.CreateTestNotification();
            var created = await _repository.CreateAsync(notification);
            var result = await _repository.GetByIdAsync(created.Id);
            Assert.That(result, Is.Not.Null);
        }
        
        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(999);
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task GetByRecipientAsync_ReturnsUserNotifications()
        {
            await _repository.CreateAsync(TestDataBuilder.CreateTestNotification(1));
            await _repository.CreateAsync(TestDataBuilder.CreateTestNotification(1));
            var result = await _repository.GetByRecipientAsync(1);
            Assert.That(result.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            await _repository.CreateAsync(TestDataBuilder.CreateTestNotification(1));
            var count = await _repository.GetUnreadCountAsync(1);
            Assert.That(count, Is.EqualTo(1));
        }
        
        [Test]
        public async Task MarkAsReadAsync_UpdatesNotificationStatus()
        {
            var notification = TestDataBuilder.CreateTestNotification(1);
            var created = await _repository.CreateAsync(notification);
            var result = await _repository.MarkAsReadAsync(created.Id, 1);
            Assert.That(result, Is.True);
        }
        
        [Test]
        public async Task DeleteAsync_ExistingNotification_RemovesNotification()
        {
            var notification = TestDataBuilder.CreateTestNotification(1);
            var created = await _repository.CreateAsync(notification);
            var result = await _repository.DeleteAsync(created.Id, 1);
            Assert.That(result, Is.True);
        }
    }
}
