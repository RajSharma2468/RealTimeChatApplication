using Microsoft.EntityFrameworkCore;
using ConnectHub.Admin.Models;

namespace ConnectHub.Admin.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
        {
        }
        
        public DbSet<AuditLog> AuditLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Index for faster queries
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.AdminId);
            
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.CreatedAt);
            
            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.Action);
        }
    }
}