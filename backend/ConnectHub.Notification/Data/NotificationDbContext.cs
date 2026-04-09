using Microsoft.EntityFrameworkCore;
using ConnectHub.Notification.Models;

namespace ConnectHub.Notification.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }
        
        public DbSet<NotificationEntity> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<NotificationEntity>()
                .HasIndex(n => n.RecipientId);
            
            modelBuilder.Entity<NotificationEntity>()
                .HasIndex(n => new { n.RecipientId, n.IsRead });
        }
    }
}
