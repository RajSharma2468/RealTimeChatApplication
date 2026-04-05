using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConnectHub.Messaging.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MessageDbContext>
    {
        public MessageDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessageDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=ConnectHubDB;Username=postgres;Password=123456");
            return new MessageDbContext(optionsBuilder.Options);
        }
    }
}
