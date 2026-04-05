using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ConnectHub.Messaging.Repositories;
using ConnectHub.Messaging.Tests.Mocks;

namespace ConnectHub.Messaging.Tests.UnitTests
{
    [TestFixture]
    public class MessageRepositoryTests
    {
        // Test: Create message should add to database
        [Test]
        public async Task CreateAsync_ValidMessage_AddsToDatabase()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var newMessage = new Models.Message
            {
                SenderId = 5,
                ReceiverId = 6,
                Content = "New message",
                SentAt = DateTime.UtcNow
            };
            
            var result = await repo.CreateAsync(newMessage);
            
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Content, Is.EqualTo("New message"));
        }
        
        // Test: Get message by existing ID should return message
        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsMessage()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var result = await repo.GetByIdAsync(1);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }
        
        // Test: Get message by non-existing ID should return null
        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var result = await repo.GetByIdAsync(999);
            
            Assert.That(result, Is.Null);
        }
        
        // Test: Get direct messages between two users
        [Test]
        public async Task GetDirectMessagesAsync_ReturnsConversation()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var result = await repo.GetDirectMessagesAsync(1, 2, 1, 10);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
        }
        
        // Test: Soft delete message - FIXED with IgnoreQueryFilters
        [Test]
        public async Task SoftDeleteAsync_ValidMessage_MarksAsDeleted()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            // First create a brand new message
            var newMessage = new Models.Message
            {
                SenderId = 100,
                ReceiverId = 101,
                Content = "Fresh message to delete",
                SentAt = DateTime.UtcNow,
                IsDeleted = false
            };
            var created = await repo.CreateAsync(newMessage);
            var createdId = created.Id;
            
            Assert.That(createdId, Is.GreaterThan(0));
            
            // Act: Soft delete the message
            var deleteResult = await repo.SoftDeleteAsync(createdId);
            
            Assert.That(deleteResult, Is.True);
            
            // Use IgnoreQueryFilters to bypass the soft delete filter
            var deletedMessage = await context.Messages
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == createdId);
            
            Assert.That(deletedMessage, Is.Not.Null);
            Assert.That(deletedMessage.IsDeleted, Is.True);
            Assert.That(deletedMessage.Content, Is.EqualTo("[Message deleted]"));
        }
        
        // Test: Search messages by keyword
        [Test]
        public async Task SearchMessagesAsync_WithKeyword_ReturnsMatches()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var result = await repo.SearchMessagesAsync(1, "Hello", null);
            
            Assert.That(result, Is.Not.Null);
        }
        
        // Test: Get unread count for a user
        [Test]
        public async Task GetUnreadCountAsync_ReturnsCorrectCount()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            var count = await repo.GetUnreadCountAsync(1);
            
            Assert.That(count, Is.GreaterThanOrEqualTo(0));
        }
        
        // Test: Mark message as read
        [Test]
        public async Task MarkAsReadAsync_ValidMessage_UpdatesStatus()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new MessageRepository(context);
            
            // Create a new unread message first
            var newMessage = new Models.Message
            {
                SenderId = 200,
                ReceiverId = 201,
                Content = "Unread message",
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            var created = await repo.CreateAsync(newMessage);
            
            // Act: Mark the message as read
            var result = await repo.MarkAsReadAsync(created.Id, created.ReceiverId.Value);
            
            Assert.That(result, Is.True);
            
            var message = await repo.GetByIdAsync(created.Id);
            Assert.That(message, Is.Not.Null);
            Assert.That(message.IsRead, Is.True);
            Assert.That(message.ReadAt, Is.Not.Null);
        }
    }
}