using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using ConnectHub.Messaging.Controllers;
using ConnectHub.Messaging.DTOs;
using ConnectHub.Messaging.Services;

namespace ConnectHub.Messaging.Tests.UnitTests
{
    /// Tests for MessageController - API endpoint logic
    [TestFixture]
    public class MessageControllerTests
    {
        private Mock<IMessageService> _mockService;
        private MessageController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IMessageService>();
            _controller = new MessageController(_mockService.Object);
            
            // IMPORTANT: Mock the User Claims (JWT token se userId aata hai)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")  // UserId = 1
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        // Test: Send message returns 200 OK
        [Test]
        public async Task SendDirectMessage_ValidData_ReturnsOk()
        {
            var dto = new SendMessageDto { ReceiverId = 2, Content = "Hello" };
            var response = new MessageResponseDto { Id = 10, Content = "Hello" };
            
            _mockService.Setup(x => x.SendDirectMessageAsync(1, dto)).ReturnsAsync(response);
            
            var result = await _controller.SendDirectMessage(dto);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Get direct messages returns Ok
        [Test]
        public async Task GetDirectMessages_ValidRequest_ReturnsOk()
        {
            var messages = new List<MessageResponseDto>();
            _mockService.Setup(x => x.GetDirectMessagesAsync(1, 2, 1, 20)).ReturnsAsync(messages);
            
            var result = await _controller.GetDirectMessages(2, 1, 20);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Delete message returns Ok
        [Test]
        public async Task DeleteMessage_ValidRequest_ReturnsOk()
        {
            _mockService.Setup(x => x.DeleteMessageAsync(1, 5)).ReturnsAsync(true);
            
            var result = await _controller.DeleteMessage(5);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Search messages returns Ok
        [Test]
        public async Task SearchMessages_WithKeyword_ReturnsOk()
        {
            var results = new List<SearchMessageDto>();
            _mockService.Setup(x => x.SearchMessagesAsync(1, "test", null)).ReturnsAsync(results);
            
            var result = await _controller.SearchMessages("test", null);
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        // Test: Get unread count returns Ok
        [Test]
        public async Task GetUnreadCount_ReturnsOk()
        {
            _mockService.Setup(x => x.GetUnreadCountAsync(1)).ReturnsAsync(3);
            
            var result = await _controller.GetUnreadCount();
            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}