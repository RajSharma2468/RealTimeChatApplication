using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ConnectHub.Auth.Helpers;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Tests.UnitTests
{
    /// Unit Tests for JwtHelper (JWT Token generation and validation)
    /// DotNet Concept: JWT, Cryptography, Configuration

    [TestFixture]
    public class JwtHelperTests
    {
        private JwtHelper _jwtHelper;
        private Mock<IConfiguration> _mockConfig;
        

        /// Setup runs before each test
        /// Creates fake configuration with JWT settings
    
        [SetUp]
        public void Setup()
        {
            _mockConfig = new Mock<IConfiguration>();
            
            // Setup fake configuration values
            _mockConfig.Setup(x => x["Jwt:Key"]).Returns("ThisIsA32ByteLongSecretKeyForTesting!");
            _mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("ConnectHub");
            _mockConfig.Setup(x => x["Jwt:Audience"]).Returns("ConnectHubClient");
            
            _jwtHelper = new JwtHelper(_mockConfig.Object);
        }
        

        /// Test: GenerateToken should return a valid JWT string
        /// JWT contains: Header, Payload, Signature
    
        [Test]
        public void GenerateToken_ValidUser_ReturnsToken()
        {
            // ========== ARRANGE ==========
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                DisplayName = "Test User"
            };
            
            // ========== ACT ==========
            var token = _jwtHelper.GenerateToken(user);
            
            // ========== ASSERT ==========
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
            // JWT tokens have 3 parts separated by dots
            Assert.That(token.Split('.').Length, Is.EqualTo(3));
        }
        

        /// Test: ValidateToken should extract UserId from valid token
    
        [Test]
        public void ValidateToken_ValidToken_ReturnsUserId()
        {
            // ========== ARRANGE ==========
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                DisplayName = "Test User"
            };
            
            // First generate a token
            var token = _jwtHelper.GenerateToken(user);
            
            // ========== ACT ==========
            // Then validate it and extract UserId
            var userId = _jwtHelper.ValidateToken(token);
            
            // ========== ASSERT ==========
            Assert.That(userId, Is.EqualTo(1));
        }
        

        /// Test: ValidateToken should return null for invalid token
    
        [Test]
        public void ValidateToken_InvalidToken_ReturnsNull()
        {
            // ========== ACT ==========
            var userId = _jwtHelper.ValidateToken("invalid_token_string");
            
            // ========== ASSERT ==========
            Assert.That(userId, Is.Null);
        }
        

        /// Test: ValidateToken should return null for empty token
    
        [Test]
        public void ValidateToken_EmptyToken_ReturnsNull()
        {
            // ========== ACT ==========
            var userId = _jwtHelper.ValidateToken("");
            
            // ========== ASSERT ==========
            Assert.That(userId, Is.Null);
        }
    }
}