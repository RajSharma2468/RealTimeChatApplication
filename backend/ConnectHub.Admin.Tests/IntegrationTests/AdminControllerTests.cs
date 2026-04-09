using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using ConnectHub.Admin.Controllers;
using ConnectHub.Admin.Services;
using ConnectHub.Admin.DTOs;
using ConnectHub.Admin.Tests.Helpers;
using ConnectHub.Admin.Tests.Mocks;

namespace ConnectHub.Admin.Tests.IntegrationTests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private MockAdminRepository _mockRepo;
        private AdminService _service;
        private AdminController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockAdminRepository();
            _mockRepo.Reset();
            _service = new AdminService(_mockRepo);
            _controller = new AdminController(_service);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        [Test]
        public async Task GetAllUsers_ReturnsOk()
        {
            var result = await _controller.GetAllUsers();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetUserById_ExistingId_ReturnsOk()
        {
            var result = await _controller.GetUserById(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            var result = await _controller.GetUserById(999);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        
        [Test]
        public async Task SuspendUser_ValidId_ReturnsOk()
        {
            var result = await _controller.SuspendUser(2, "Spamming");
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            var result = await _controller.DeleteUser(2);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetAllRooms_ReturnsOk()
        {
            var result = await _controller.GetAllRooms();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteRoom_ValidId_ReturnsOk()
        {
            var result = await _controller.DeleteRoom(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetAllMessages_ReturnsOk()
        {
            var result = await _controller.GetAllMessages();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteMessage_ValidId_ReturnsOk()
        {
            var result = await _controller.DeleteMessage(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetAnalytics_ReturnsOk()
        {
            var result = await _controller.GetAnalytics();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetAuditLogs_ReturnsOk()
        {
            var result = await _controller.GetAuditLogs(1, 50);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}