using Moq;
using NUnit.Framework;
using ConnectHub.Auth.Models;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Services;
using ConnectHub.Auth.Repositories;
using ConnectHub.Auth.Helpers;

namespace ConnectHub.Auth.Tests.UnitTests
{
    /// Unit Tests for UserService
    /// DotNet Concept: Unit Testing, Mocking, AAA Pattern (Arrange-Act-Assert)

    [TestFixture]  // Marks this class as containing NUnit tests
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepo;  // Fake repository
        private Mock<IJwtHelper> _mockJwt;        // Fake JWT helper
        private UserService _userService;         // System Under Test (SUT)
        

        /// Setup runs before EVERY test
        /// Creates fresh mocks and service instance
        /// DotNet Concept: Test Initialization
    
        [SetUp]
        public void Setup()
        {
            // Create fake objects (Mocks)
            _mockRepo = new Mock<IUserRepository>();
            _mockJwt = new Mock<IJwtHelper>();
            
            // Inject fake dependencies into real service
            // This is Dependency Injection in tests
            _userService = new UserService(_mockRepo.Object, _mockJwt.Object);
        }
        
        // ========== REGISTER TESTS ==========
        

        /// Test: Register with valid data should succeed
        /// AAA Pattern: Arrange, Act, Assert
    
        [Test]  // Marks this method as a test
        public async Task RegisterAsync_ValidUser_ReturnsUserResponseDto()
        {
            // ========== ARRANGE ==========
            // Setup test data
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                DisplayName = "New User",
                Email = "newuser@example.com",
                Password = "Test123456"
            };
            
            // Setup mocks - tell them what to return
            // If UsernameExistsAsync is called, return false (username not taken)
            _mockRepo.Setup(x => x.UsernameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            // If EmailExistsAsync is called, return false (email not taken)
            _mockRepo.Setup(x => x.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            // If CreateAsync is called, return a new user with Id=10
            _mockRepo.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 10 });
            
            // ========== ACT ==========
            // Call the actual method we want to test
            var result = await _userService.RegisterAsync(registerDto);
            
            // ========== ASSERT ==========
            // Verify results
            Assert.That(result, Is.Not.Null);  // Result should not be null
            Assert.That(result.Username, Is.EqualTo("newuser"));  // Username matches
            
            // Verify that CreateAsync was called exactly once
            _mockRepo.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        }
        

        /// Test: Register with existing username should throw exception
    
        [Test]
        public void RegisterAsync_ExistingUsername_ThrowsException()
        {
            // ========== ARRANGE ==========
            var registerDto = new RegisterDto
            {
                Username = "existing",
                DisplayName = "Test",
                Email = "test@example.com",
                Password = "Test123456"
            };
            
            // Mock says: username already exists
            _mockRepo.Setup(x => x.UsernameExistsAsync("existing")).ReturnsAsync(true);
            
            // ========== ACT & ASSERT ==========
            // Expect an Exception to be thrown
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _userService.RegisterAsync(registerDto));
            
            // Verify exception message is correct
            Assert.That(ex.Message, Is.EqualTo("Username already exists"));
        }
        
        // ========== LOGIN TESTS ==========
        

        /// Test: Valid login credentials should return JWT token
    
        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // ========== ARRANGE ==========
            var loginDto = new LoginDto
            {
                Username = "john_doe",
                Password = "Password123"
            };
            
            // Create a fake user that matches the database
            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                DisplayName = "John Doe",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                IsActive = true
            };
            
            // Mock repository to return this user
            _mockRepo.Setup(x => x.GetByUsernameOrEmailAsync("john_doe")).ReturnsAsync(user);
            // Mock JWT helper to return a fake token
            _mockJwt.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("fake_jwt_token");
            
            // ========== ACT ==========
            var result = await _userService.LoginAsync(loginDto);
            
            // ========== ASSERT ==========
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Token, Is.EqualTo("fake_jwt_token"));  // Token matches
            Assert.That(result.UserId, Is.EqualTo(1));  // User ID matches
            Assert.That(result.Username, Is.EqualTo("john_doe"));  // Username matches
        }
        

        /// Test: Wrong password should throw exception
    
        [Test]
        public void LoginAsync_InvalidPassword_ThrowsException()
        {
            // ========== ARRANGE ==========
            var loginDto = new LoginDto
            {
                Username = "john_doe",
                Password = "WrongPassword"  // Wrong password
            };
            
            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
                IsActive = true
            };
            
            _mockRepo.Setup(x => x.GetByUsernameOrEmailAsync("john_doe")).ReturnsAsync(user);
            
            // ========== ACT & ASSERT ==========
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _userService.LoginAsync(loginDto));
            
            Assert.That(ex.Message, Is.EqualTo("Invalid username or password"));
        }
        

        /// Test: Deactivated account should not be allowed to login
    
        [Test]
        public void LoginAsync_InactiveAccount_ThrowsException()
        {
            // ========== ARRANGE ==========
            var loginDto = new LoginDto
            {
                Username = "inactive_user",
                Password = "Password123"
            };
            
            // User with IsActive = false
            var user = new User
            {
                Id = 3,
                Username = "inactive_user",
                IsActive = false  // Account is deactivated
            };
            
            _mockRepo.Setup(x => x.GetByUsernameOrEmailAsync("inactive_user")).ReturnsAsync(user);
            
            // ========== ACT & ASSERT ==========
            var ex = Assert.ThrowsAsync<Exception>(async () => 
                await _userService.LoginAsync(loginDto));
            
            Assert.That(ex.Message, Is.EqualTo("Account is deactivated. Contact admin."));
        }
        
        // ========== PROFILE TESTS ==========
        

        /// Test: Get profile should return user data
    
        [Test]
        public async Task GetProfileAsync_ValidUserId_ReturnsUserProfile()
        {
            // ========== ARRANGE ==========
            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                DisplayName = "John Doe",
                Email = "john@example.com",
                Bio = "Software Developer",
                IsActive = true
            };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            
            // ========== ACT ==========
            var result = await _userService.GetProfileAsync(1);
            
            // ========== ASSERT ==========
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Bio, Is.EqualTo("Software Developer"));
        }
        

        /// Test: Update profile should change user data
    
        [Test]
        public async Task UpdateProfileAsync_ValidData_UpdatesProfile()
        {
            // ========== ARRANGE ==========
            var user = new User
            {
                Id = 1,
                Username = "john_doe",
                DisplayName = "John Doe",
                Bio = "Old Bio"
            };
            
            var updateDto = new UpdateProfileDto
            {
                DisplayName = "John Updated",
                Bio = "New Bio"
            };
            
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);
            
            // ========== ACT ==========
            var result = await _userService.UpdateProfileAsync(1, updateDto);
            
            // ========== ASSERT ==========
            // Verify that values were updated
            Assert.That(result.DisplayName, Is.EqualTo("John Updated"));
            Assert.That(result.Bio, Is.EqualTo("New Bio"));
        }
        
        // ========== SEARCH TESTS ==========
        

        /// Test: Search users should return matching users
    
        [Test]
        public async Task SearchUsersAsync_WithKeyword_ReturnsMatchingUsers()
        {
            // ========== ARRANGE ==========
            var users = new List<User>
            {
                new User { Id = 2, Username = "jane_smith", DisplayName = "Jane Smith" },
                new User { Id = 3, Username = "test_user", DisplayName = "Test User" }
            };
            
            _mockRepo.Setup(x => x.SearchUsersAsync("jane", 1)).ReturnsAsync(users);
            
            // ========== ACT ==========
            var result = await _userService.SearchUsersAsync("jane", 1);
            
            // ========== ASSERT ==========
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));  // Should return 2 users
        }
    }
}