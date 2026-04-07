using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ConnectHub.Auth.Controllers;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Services;

namespace ConnectHub.Auth.Tests.IntegrationTests
{
    /// Integration Tests for AuthController
    /// Tests the complete API endpoint flow
    /// DotNet Concept: Controller Testing, HTTP Responses, API Integration

    [TestFixture] /// Marks class containing tests
    public class AuthControllerTests
    {
        private Mock<IUserService> _mockService;
        private AuthController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IUserService>();
            _controller = new AuthController(_mockService.Object);
        }
        

        /// Test: POST /api/auth/register should return 200 OK with user data
    
        [Test]
        public async Task Register_ValidData_ReturnsOk()
        {
            // ========== ARRANGE ==========
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                DisplayName = "New User",
                Email = "new@example.com",
                Password = "Test123456"
            };
            
            var expectedResponse = new UserResponseDto
            {
                Id = 10,
                Username = "newuser",
                DisplayName = "New User"
            };
            
            _mockService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(expectedResponse);
            
            // ========== ACT ==========
            var result = await _controller.Register(registerDto);
            
            // ========== ASSERT ==========
            // Verify HTTP 200 OK response
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        

        /// Test: POST /api/auth/login should return 200 OK with JWT token
    
        [Test]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // ========== ARRANGE ==========
            var loginDto = new LoginDto
            {
                Username = "john_doe",
                Password = "Password123"
            };
            
            var expectedResponse = new LoginResponseDto
            {
                Token = "fake_jwt_token",
                UserId = 1,
                Username = "john_doe",
                DisplayName = "John Doe",
                Message = "Login successful"
            };
            
            _mockService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(expectedResponse);
            
            // ========== ACT ==========
            var result = await _controller.Login(loginDto);
            
            // ========== ASSERT ==========
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
        

        /// Test: POST /api/auth/login with wrong credentials should return 401 Unauthorized
    
        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // ========== ARRANGE ==========
            var loginDto = new LoginDto
            {
                Username = "wrong",
                Password = "wrong"
            };
            
            // Service throws exception for invalid credentials
            _mockService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Invalid username or password"));
            
            // ========== ACT ==========
            var result = await _controller.Login(loginDto);
            
            // ========== ASSERT ==========
            // Verify HTTP 401 Unauthorized response
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.That(unauthorizedResult, Is.Not.Null);
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
        }
    }
}