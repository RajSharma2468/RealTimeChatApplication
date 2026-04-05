using Microsoft.EntityFrameworkCore;
using ConnectHub.Auth.Data;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Tests.Fixtures
{
    /// Database Fixture - Shared database setup for all tests
    /// Runs once before all tests, disposed after all tests

    [SetUpFixture]
    public class DatabaseFixture
    {
        private AppDbContext _context;
        

        /// Runs once before ALL tests in the assembly
        /// Creates a shared database for integration tests
    
        [OneTimeSetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "SharedTestDatabase")
                .Options;
            
            _context = new AppDbContext(options);
            
            // Seed common test data
            SeedDatabase();
        }
        
        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User { Id = 1, Username = "admin", DisplayName = "Admin User", 
                          Email = "admin@example.com", IsActive = true,
                          PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123") },
                new User { Id = 2, Username = "testuser1", DisplayName = "Test User 1", 
                          Email = "test1@example.com", IsActive = true,
                          PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123") },
                new User { Id = 3, Username = "testuser2", DisplayName = "Test User 2", 
                          Email = "test2@example.com", IsActive = true,
                          PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123") }
            };
            
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }
        

        /// Returns the shared database context
    
        public AppDbContext GetContext() => _context;
        

        /// Clears all data from database (for test isolation)
    
        public void ClearDatabase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }
        

        /// Runs once after ALL tests complete
    
        [OneTimeTearDown]
        public void Teardown()
        {
            _context?.Dispose();
        }
    }
}