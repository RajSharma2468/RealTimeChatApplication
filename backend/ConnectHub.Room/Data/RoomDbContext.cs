using Microsoft.EntityFrameworkCore;
using ConnectHub.Room.Models;

namespace ConnectHub.Room.Data
{
    public class RoomDbContext : DbContext
    {
        public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
        {
        }
        
        // Database tables
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<RoomMember> RoomMembers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Unique constraint: One user cannot join same room twice
            modelBuilder.Entity<RoomMember>()
                .HasIndex(rm => new { rm.RoomId, rm.UserId })
                .IsUnique();
            
            // Index for faster queries
            modelBuilder.Entity<RoomMember>()
                .HasIndex(rm => rm.UserId);
            
            modelBuilder.Entity<ChatRoom>()
                .HasIndex(cr => cr.CreatedBy);
            
            // Soft delete filter - hide inactive rooms by default
            modelBuilder.Entity<ChatRoom>()
                .HasQueryFilter(cr => cr.IsActive);
            
            modelBuilder.Entity<RoomMember>()
                .HasQueryFilter(rm => rm.IsActive);
        }
    }
}