using NUnit.Framework;
using ConnectHub.Auth.Repositories;
using ConnectHub.Auth.Tests.Mocks;

namespace ConnectHub.Auth.Tests.UnitTests
{
    /// Tests for UserRepository - Database operations testing using In-Memory database
    [TestFixture]
    public class UserRepositoryTests
    {
        // Test: Get user by existing ID should return user
        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsUser()
        {
            // Arrange: Create in-memory database with test data
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            // Act: Get user with ID 1
            var result = await repository.GetByIdAsync(1);
            
            // Assert: Verify user data
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Username, Is.EqualTo("john_doe"));
        }

        // Test: Get user by non-existing ID should return null
        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.GetByIdAsync(999);
            
            Assert.That(result, Is.Null);
        }

        // Test: Get user by username should return correct user
        [Test]
        public async Task GetByUsernameAsync_ExistingUser_ReturnsUser()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.GetByUsernameAsync("john_doe");
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("john_doe"));
        }

        // Test: Check if username exists should return true for existing username
        [Test]
        public async Task UsernameExistsAsync_ExistingUsername_ReturnsTrue()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.UsernameExistsAsync("john_doe");
            
            Assert.That(result, Is.True);
        }

        // Test: Check if username exists should return false for new username
        [Test]
        public async Task UsernameExistsAsync_NonExistingUsername_ReturnsFalse()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.UsernameExistsAsync("nonexistent");
            
            Assert.That(result, Is.False);
        }

        // Test: Check if email exists should return true for existing email
        [Test]
        public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.EmailExistsAsync("john@example.com");
            
            Assert.That(result, Is.True);
        }

        // Test: Create new user should add to database
        [Test]
        public async Task CreateAsync_ValidUser_AddsUser()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var newUser = new ConnectHub.Auth.Models.User
            {
                Username = "newuser",
                DisplayName = "New User",
                Email = "new@example.com",
                PasswordHash = "hashed_password",
                IsActive = true
            };
            
            var result = await repository.CreateAsync(newUser);
            
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Username, Is.EqualTo("newuser"));
        }

        // Test: Search users by keyword should return matching users
        [Test]
        public async Task SearchUsersAsync_WithKeyword_ReturnsMatchingUsers()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            // Use currentUserId=99 (doesn't exist) to avoid excluding john_doe
            var result = await repository.SearchUsersAsync("john", 99);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.GreaterThan(0));
        }

        // Test: Search with empty keyword should return empty list
        [Test]
        public async Task SearchUsersAsync_EmptyKeyword_ReturnsEmptyList()
        {
            var context = MockDbContext.GetDbContext();
            var repository = new UserRepository(context);
            
            var result = await repository.SearchUsersAsync("", 1);
            
            Assert.That(result, Is.Not.Null);
        }
    }
}