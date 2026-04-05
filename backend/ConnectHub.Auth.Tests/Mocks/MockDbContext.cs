using Microsoft.EntityFrameworkCore;
using ConnectHub.Auth.Data;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Tests.Mocks
{
    /// Mock Database Context - Creates fake in-memory database for testing
    /// DotNet Concept: InMemory Database, Seeding Data, F
    public static class MockDbContext
    {
        /// Returns a fresh in-memory database context
        /// Each test gets its own isolated database

        public static AppDbContext GetDbContext()
        {
            // Create unique database name for each test
            // Guid.NewGuid() ensures no two tests share the same database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new AppDbContext(options);
            
            // Seed (add) test data to the in-memory database
            context.Users.AddRange(GetTestUsers());
            context.SaveChanges();
            
            return context;
        }
        
        /// Pre-defined test users
        /// These act as existing data before each test runs

        private static List<User> GetTestUsers()
        {
            return new List<User>
            {
                // Active user for login tests
                new User
                {
                    Id = 1,
                    Username = "john_doe",
                    DisplayName = "John Doe",
                    Email = "john@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                // Second active user for chat tests
                new User
                {
                    Id = 2,
                    Username = "jane_smith",
                    DisplayName = "Jane Smith",
                    Email = "jane@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                // Inactive user - should not be able to login
                new User
                {
                    Id = 3,
                    Username = "inactive_user",
                    DisplayName = "Inactive User",
                    Email = "inactive@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                    IsActive = false,  // This user is deactivated
                    CreatedAt = DateTime.UtcNow
                }
            };
        }
    }
}