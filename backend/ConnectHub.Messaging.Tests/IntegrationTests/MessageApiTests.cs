using NUnit.Framework;
using ConnectHub.Messaging.Tests.Mocks;

namespace ConnectHub.Messaging.Tests.IntegrationTests
{
    /// Integration tests for complete message flow
    [TestFixture]
    public class MessageApiTests
    {
        // Test: Full message lifecycle (send → edit → delete)
        [Test]
        public async Task MessageLifecycle_CreateEditDelete_WorksCorrectly()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.MessageRepository(context);
            var service = new Services.MessageService(repo);
            
            // Step 1: Send message
            var dto = new DTOs.SendMessageDto { ReceiverId = 2, Content = "Original message" };
            var sent = await service.SendDirectMessageAsync(1, dto);
            Assert.That(sent.Content, Is.EqualTo("Original message"));
            
            // Step 2: Edit message
            var editDto = new DTOs.EditMessageDto { MessageId = sent.Id, NewContent = "Edited message" };
            var edited = await service.EditMessageAsync(1, editDto);
            Assert.That(edited.IsEdited, Is.True);
            Assert.That(edited.Content, Is.EqualTo("Edited message"));
            
            // Step 3: Delete message
            var deleted = await service.DeleteMessageAsync(1, sent.Id);
            Assert.That(deleted, Is.True);
        }
        
        // Test: Get conversation between two users
        [Test]
        public async Task GetConversation_ReturnsMessageHistory()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.MessageRepository(context);
            var service = new Services.MessageService(repo);
            
            // Send multiple messages
            await service.SendDirectMessageAsync(1, new DTOs.SendMessageDto { ReceiverId = 2, Content = "Msg 1" });
            await service.SendDirectMessageAsync(2, new DTOs.SendMessageDto { ReceiverId = 1, Content = "Msg 2" });
            await service.SendDirectMessageAsync(1, new DTOs.SendMessageDto { ReceiverId = 2, Content = "Msg 3" });
            
            var history = await service.GetDirectMessagesAsync(1, 2, 1, 10);
            
            Assert.That(history.Count(), Is.GreaterThanOrEqualTo(3));
        }
        
        // Test: Search messages
        [Test]
        public async Task SearchMessages_FindsMatchingContent()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.MessageRepository(context);
            var service = new Services.MessageService(repo);
            
            await service.SendDirectMessageAsync(1, new DTOs.SendMessageDto { ReceiverId = 2, Content = "Meeting at 3pm" });
            await service.SendDirectMessageAsync(1, new DTOs.SendMessageDto { ReceiverId = 2, Content = "Lunch break" });
            
            var results = await service.SearchMessagesAsync(1, "Meeting", null);
            
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Content, Does.Contain("Meeting"));
        }
        
        // Test: Mark message as read - FIXED
        [Test]
        public async Task MarkAsRead_UpdatesMessageStatus()
        {
            var context = MockDbContext.GetDbContext();
            var repo = new Repositories.MessageRepository(context);
            var service = new Services.MessageService(repo);
            
            // Send a new message from user 1 to user 2
            var sent = await service.SendDirectMessageAsync(1, new DTOs.SendMessageDto { ReceiverId = 2, Content = "Read this" });
            
            // Mark as read by user 2 (receiver)
            var marked = await service.MarkAsReadAsync(2, sent.Id);
            
            Assert.That(marked, Is.True);
            
            // Verify this specific message is marked as read
            var message = await repo.GetByIdAsync(sent.Id);
            Assert.That(message.IsRead, Is.True);
            Assert.That(message.ReadAt, Is.Not.Null);
        }
    }
}