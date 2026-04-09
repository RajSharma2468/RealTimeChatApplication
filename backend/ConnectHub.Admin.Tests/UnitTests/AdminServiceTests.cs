using Moq;
using NUnit.Framework;
using ConnectHub.Admin.Models;
using ConnectHub.Admin.DTOs;
using ConnectHub.Admin.Services;
using ConnectHub.Admin.Repositories;
using ConnectHub.Admin.Tests.Helpers;
using ConnectHub.Admin.Tests.Mocks;

namespace ConnectHub.Admin.Tests.UnitTests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private MockAdminRepository _mockRepo;
        private AdminService _service;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockAdminRepository();
            _mockRepo.Reset();
            _service = new AdminService(_mockRepo);
        }
        
        [Test]
        public async Task GetAllUsersAsync_ReturnsUsers()
        {
            var result = await _service.GetAllUsersAsync();
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.First().Username, Is.EqualTo("admin"));
        }
        
        [Test]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            var result = await _service.GetUserByIdAsync(1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }
        
        [Test]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            var result = await _service.GetUserByIdAsync(999);
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task SuspendUserAsync_CreatesAuditLog()
        {
            var result = await _service.SuspendUserAsync(1, 2, "Spamming");
            Assert.That(result, Is.True);
            
            var logs = await _mockRepo.GetAuditLogsAsync();
            Assert.That(logs.Count(), Is.EqualTo(1));
            Assert.That(logs.First().Action, Is.EqualTo("SUSPEND_USER"));
        }
        
        [Test]
        public async Task DeleteUserAsync_CreatesAuditLog()
        {
            var result = await _service.DeleteUserAsync(1, 2);
            Assert.That(result, Is.True);
            
            var logs = await _mockRepo.GetAuditLogsAsync();
            Assert.That(logs.Count(), Is.EqualTo(1));
            Assert.That(logs.First().Action, Is.EqualTo("DELETE_USER"));
        }
        
        [Test]
        public async Task GetAllRoomsAsync_ReturnsRooms()
        {
            var result = await _service.GetAllRoomsAsync();
            Assert.That(result.Count(), Is.EqualTo(3));
        }
        
        [Test]
        public async Task DeleteRoomAsync_CreatesAuditLog()
        {
            var result = await _service.DeleteRoomAsync(1, 1);
            Assert.That(result, Is.True);
            
            var logs = await _mockRepo.GetAuditLogsAsync();
            Assert.That(logs.First().Action, Is.EqualTo("DELETE_ROOM"));
        }
        
        [Test]
        public async Task GetAllMessagesAsync_ReturnsMessages()
        {
            var result = await _service.GetAllMessagesAsync();
            Assert.That(result.Count(), Is.EqualTo(3));
        }
        
        [Test]
        public async Task DeleteMessageAsync_CreatesAuditLog()
        {
            var result = await _service.DeleteMessageAsync(1, 1);
            Assert.That(result, Is.True);
            
            var logs = await _mockRepo.GetAuditLogsAsync();
            Assert.That(logs.First().Action, Is.EqualTo("DELETE_MESSAGE"));
        }
        
        [Test]
        public async Task GetAnalyticsAsync_ReturnsAnalytics()
        {
            var result = await _service.GetAnalyticsAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalUsers, Is.EqualTo(150));
            Assert.That(result.TotalMessages, Is.EqualTo(12500));
            Assert.That(result.TotalRooms, Is.EqualTo(25));
        }
        
        [Test]
        public async Task GetAuditLogsAsync_ReturnsLogs()
        {
            await _service.SuspendUserAsync(1, 2, "Test");
            await _service.DeleteRoomAsync(1, 1);
            
            var result = await _service.GetAuditLogsAsync();
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}