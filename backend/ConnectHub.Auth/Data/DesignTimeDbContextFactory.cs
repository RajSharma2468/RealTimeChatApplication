using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConnectHub.Auth.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            // Hardcoded connection string for design time
            optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=ConnectHubDB;Username=postgres;Password=123456");
            
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}