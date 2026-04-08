using Microsoft.EntityFrameworkCore;
using ConnectHub.Media.Models;

namespace ConnectHub.Media.Data
{
    public class MediaDbContext : DbContext
    {
        public MediaDbContext(DbContextOptions<MediaDbContext> options) : base(options)
        {
        }
        
        public DbSet<MediaFile> MediaFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Index for faster queries
            modelBuilder.Entity<MediaFile>()
                .HasIndex(m => m.UploadedBy);
            
            modelBuilder.Entity<MediaFile>()
                .HasIndex(m => m.MessageId);
            
            modelBuilder.Entity<MediaFile>()
                .HasIndex(m => m.ExpiresAt);
        }
    }
}