using Microsoft.EntityFrameworkCore;
using ConnectHub.Messaging.Data;
using ConnectHub.Messaging.Models;

namespace ConnectHub.Messaging.Tests.Mocks
{
    /// Creates in-memory database for messaging tests
    public static class MockDbContext
    {
        public static MessageDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<MessageDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new MessageDbContext(options);
            SeedMessages(context);
            return context;
        }
        
        private static void SeedMessages(MessageDbContext context)
        {
            var messages = new List<Message>
            {
                new Message { Id = 1, SenderId = 1, ReceiverId = 2, Content = "Hello!", SentAt = DateTime.UtcNow.AddHours(-2), IsRead = true },
                new Message { Id = 2, SenderId = 2, ReceiverId = 1, Content = "Hi back!", SentAt = DateTime.UtcNow.AddHours(-1), IsRead = false },
                new Message { Id = 3, SenderId = 1, ReceiverId = 2, Content = "How are you?", SentAt = DateTime.UtcNow, IsRead = false },
                new Message { Id = 4, SenderId = 1, ReceiverId = 3, Content = "Message to user 3", SentAt = DateTime.UtcNow.AddMinutes(-30), IsRead = false }
            };
            
            context.Messages.AddRange(messages);
            context.SaveChanges();
        }
        
        public static void AddTestMessage(MessageDbContext context, Message message)
        {
            context.Messages.Add(message);
            context.SaveChanges();
        }
    }
}