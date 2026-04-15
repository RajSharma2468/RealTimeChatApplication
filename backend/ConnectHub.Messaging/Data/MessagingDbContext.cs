using Microsoft.EntityFrameworkCore;
using ConnectHub.Messaging.Models;

namespace ConnectHub.Messaging.Data
{
    public class MessageDbContext : DbContext  
    {
        public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options) { }
        
        public DbSet<Message> Messages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.SenderId, m.ReceiverId, m.SentAt });
            
            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.RoomId, m.SentAt });
            
            modelBuilder.Entity<Message>()
                .HasQueryFilter(m => !m.IsDeleted);
        }
    }
}