using Moq;
using NUnit.Framework;
using ConnectHub.Messaging.Models;
using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Services;
using ConnectHub.Messaging.Repositories;

namespace ConnectHub.Messaging.Tests.UnitTests
{
    /// Tests for MessageService - Business logic with Moq
    [TestFixture]
    public class MessageServiceTests
    {
        private Mock<IMessageRepository> _mockRepo;
        private MessageService _service;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IMessageRepository>();
            _service = new MessageService(_mockRepo.Object);
        }
        
        // Test: Send direct message successfully
        [Test]
        public async Task SendDirectMessageAsync_ValidData_ReturnsMessage()
        {
            var dto = new SendMessageDto { ReceiverId = 2, Content = "Hello!" };
            var expectedMessage = new Message { Id = 10, SenderId = 1, ReceiverId = 2, Content = "Hello!" };
            
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<Message>())).ReturnsAsync(expectedMessage);
            
            var result = await _service.SendDirectMessageAsync(1, dto);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.EqualTo("Hello!"));
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<Message>()), Times.Once);
        }
        
        // Test: Edit own message successfully
        [Test]
        public async Task EditMessageAsync_OwnMessage_UpdatesContent()
        {
            var editDto = new EditMessageDto { MessageId = 1, NewContent = "Updated!" };
            var existingMessage = new Message { Id = 1, SenderId = 1, Content = "Old", IsDeleted = false };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingMessage);
            _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<Message>())).ReturnsAsync(existingMessage);
            
            var result = await _service.EditMessageAsync(1, editDto);
            
            Assert.That(result.IsEdited, Is.True);
            Assert.That(result.Content, Is.EqualTo("Updated!"));
        }
        
        // Test: Cannot edit someone else's message
        [Test]
        public void EditMessageAsync_NotOwnMessage_ThrowsException()
        {
            var editDto = new EditMessageDto { MessageId = 1, NewContent = "Updated!" };
            var existingMessage = new Message { Id = 1, SenderId = 2, IsDeleted = false };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingMessage);
            
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.EditMessageAsync(1, editDto));
            Assert.That(ex.Message, Is.EqualTo("You can only edit your own messages"));
        }
        
        // Test: Cannot edit deleted message
        [Test]
        public void EditMessageAsync_DeletedMessage_ThrowsException()
        {
            var editDto = new EditMessageDto { MessageId = 1, NewContent = "Updated!" };
            var existingMessage = new Message { Id = 1, SenderId = 1, IsDeleted = true };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingMessage);
            
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.EditMessageAsync(1, editDto));
            Assert.That(ex.Message, Is.EqualTo("Cannot edit deleted message"));
        }
        
        // Test: Delete own message
        [Test]
        public async Task DeleteMessageAsync_OwnMessage_SoftDeletes()
        {
            var existingMessage = new Message { Id = 1, SenderId = 1 };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingMessage);
            _mockRepo.Setup(x => x.SoftDeleteAsync(1)).ReturnsAsync(true);
            
            var result = await _service.DeleteMessageAsync(1, 1);
            
            Assert.That(result, Is.True);
        }
        
        // Test: Cannot delete someone else's message
        [Test]
        public void DeleteMessageAsync_NotOwnMessage_ThrowsException()
        {
            var existingMessage = new Message { Id = 1, SenderId = 2 };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingMessage);
            
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _service.DeleteMessageAsync(1, 1));
            Assert.That(ex.Message, Is.EqualTo("You can only delete your own messages"));
        }
        
        // Test: Get unread count
        [Test]
        public async Task GetUnreadCountAsync_ReturnsCount()
        {
            _mockRepo.Setup(x => x.GetUnreadCountAsync(1)).ReturnsAsync(5);
            
            var result = await _service.GetUnreadCountAsync(1);
            
            Assert.That(result, Is.EqualTo(5));
        }
        
        // Test: Mark message as read
        [Test]
        public async Task MarkAsReadAsync_ValidCall_ReturnsTrue()
        {
            _mockRepo.Setup(x => x.MarkAsReadAsync(1, 1)).ReturnsAsync(true);
            
            var result = await _service.MarkAsReadAsync(1, 1);
            
            Assert.That(result, Is.True);
        }
    }
}