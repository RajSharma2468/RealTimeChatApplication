using Microsoft.EntityFrameworkCore;
using ConnectHub.Messaging.Data;
using ConnectHub.Messaging.Models;

namespace ConnectHub.Messaging.Tests.Fixtures
{
    /// Database Fixture - Shared database setup for messaging tests
    [SetUpFixture]
    public class DatabaseFixture
    {
        private MessageDbContext _context;
        
        [OneTimeSetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MessageDbContext>()
                .UseInMemoryDatabase(databaseName: "MessagingTestDatabase")
                .Options;
            
            _context = new MessageDbContext(options);
            SeedDatabase();
        }
        
        private void SeedDatabase()
        {
            var messages = new List<Message>
            {
                new Message { Id = 1, SenderId = 1, ReceiverId = 2, Content = "Hello Jane!", SentAt = DateTime.UtcNow, IsRead = false },
                new Message { Id = 2, SenderId = 2, ReceiverId = 1, Content = "Hi John!", SentAt = DateTime.UtcNow, IsRead = true },
                new Message { Id = 3, SenderId = 1, ReceiverId = 2, Content = "How are you?", SentAt = DateTime.UtcNow, IsRead = false },
                new Message { Id = 4, SenderId = 3, ReceiverId = 1, Content = "Meeting at 5pm", SentAt = DateTime.UtcNow, IsRead = false },
                new Message { Id = 5, SenderId = 1, ReceiverId = 3, Content = "I'll be there", SentAt = DateTime.UtcNow, IsRead = true }
            };
            
            _context.Messages.AddRange(messages);
            _context.SaveChanges();
        }
        
        public MessageDbContext GetContext() => _context;
        
        public void ClearDatabase()
        {
            _context.Messages.RemoveRange(_context.Messages);
            _context.SaveChanges();
        }
        
        [OneTimeTearDown]
        public void Teardown()
        {
            _context?.Dispose();
        }
    }
}