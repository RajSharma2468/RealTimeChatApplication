using ConnectHub.Auth.Models;
using ConnectHub.Auth.DTOs;

namespace ConnectHub.Auth.Tests.Helpers
{
    /// Test Data Builder - Creates test objects using Builder Pattern
    /// Helps create consistent test data across all test files

    public static class TestDataBuilder
    {
        // ========== USER BUILDER ==========
        

        /// Creates a test user with default values
    
        public static User CreateTestUser(int id = 0, string username = "testuser", 
            string displayName = "Test User", string email = "test@example.com", 
            bool isActive = true)
        {
            return new User
            {
                Id = id,
                Username = username,
                DisplayName = displayName,
                Email = email,
                IsActive = isActive,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                CreatedAt = DateTime.UtcNow
            };
        }
        

        /// Creates an admin user
    
        public static User CreateAdminUser()
        {
            return CreateTestUser(id: 1, username: "admin", displayName: "Administrator", 
                email: "admin@connecthub.com", isActive: true);
        }
        

        /// Creates an inactive (deactivated) user
    
        public static User CreateInactiveUser()
        {
            return CreateTestUser(id: 99, username: "inactive", displayName: "Inactive User", 
                email: "inactive@example.com", isActive: false);
        }
        
        // ========== DTO BUILDER ==========
        

        /// Creates a valid Register DTO
    
        public static RegisterDto CreateValidRegisterDto(string username = "newuser", 
            string displayName = "New User", string email = "new@example.com", 
            string password = "Test@123456")
        {
            return new RegisterDto
            {
                Username = username,
                DisplayName = displayName,
                Email = email,
                Password = password
            };
        }
        

        /// Creates an invalid Register DTO (empty fields)
    
        public static RegisterDto CreateInvalidRegisterDto()
        {
            return new RegisterDto
            {
                Username = "",
                DisplayName = "",
                Email = "",
                Password = ""
            };
        }
        

        /// Creates a valid Login DTO
    
        public static LoginDto CreateValidLoginDto(string username = "testuser", 
            string password = "Test@123")
        {
            return new LoginDto
            {
                Username = username,
                Password = password
            };
        }
        

        /// Creates an invalid Login DTO (wrong credentials)
    
        public static LoginDto CreateInvalidLoginDto()
        {
            return new LoginDto
            {
                Username = "wronguser",
                Password = "wrongpass"
            };
        }
        

        /// Creates a valid Update Profile DTO
    
        public static UpdateProfileDto CreateUpdateProfileDto(string displayName = "Updated Name", 
            string bio = "This is my updated bio")
        {
            return new UpdateProfileDto
            {
                DisplayName = displayName,
                Bio = bio
            };
        }
        
        // ========== LIST BUILDER ==========
        

        /// Creates a list of multiple test users
    
        public static List<User> CreateMultipleTestUsers(int count = 5)
        {
            var users = new List<User>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(CreateTestUser(id: i, username: $"user{i}", 
                    displayName: $"User {i}", email: $"user{i}@example.com"));
            }
            return users;
        }
        
        // ========== SEARCH DATA BUILDER ==========
        

        /// Creates users with searchable names
    
        public static List<User> CreateSearchableUsers()
        {
            return new List<User>
            {
                CreateTestUser(id: 10, username: "john_doe", displayName: "John Doe", email: "john@example.com"),
                CreateTestUser(id: 11, username: "jane_smith", displayName: "Jane Smith", email: "jane@example.com"),
                CreateTestUser(id: 12, username: "johnny_walker", displayName: "Johnny Walker", email: "johnny@example.com"),
                CreateTestUser(id: 13, username: "peter_parker", displayName: "Peter Parker", email: "peter@example.com")
            };
        }
    }
}