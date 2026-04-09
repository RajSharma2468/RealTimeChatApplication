using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ConnectHub.Admin.Data;
using ConnectHub.Admin.Models;
using ConnectHub.Admin.Repositories;
using ConnectHub.Admin.Tests.Helpers;

namespace ConnectHub.Admin.Tests.UnitTests
{
    [TestFixture]
    public class AdminRepositoryTests
    {
        private AdminDbContext _context;
        private AdminRepository _repository;
        
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AdminDbContext(options);
            _repository = new AdminRepository(_context);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
        
        [Test]
        public async Task CreateAuditLogAsync_ValidLog_AddsToDatabase()
        {
            var log = TestDataBuilder.CreateTestAuditLog();
            var result = await _repository.CreateAuditLogAsync(log);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Action, Is.EqualTo("SUSPEND_USER"));
        }
        
        [Test]
        public async Task GetAuditLogsAsync_ReturnsLogs()
        {
            await _repository.CreateAuditLogAsync(TestDataBuilder.CreateTestAuditLog());
            await _repository.CreateAuditLogAsync(TestDataBuilder.CreateTestAuditLog(action: "DELETE_ROOM"));
            
            var result = await _repository.GetAuditLogsAsync();
            Assert.That(result.Count(), Is.EqualTo(2));
        }
        
        [Test]
        public async Task GetTotalUsersAsync_ReturnsCount()
        {
            var count = await _repository.GetTotalUsersAsync();
            Assert.That(count, Is.EqualTo(150));
        }
        
        [Test]
        public async Task GetTotalMessagesAsync_ReturnsCount()
        {
            var count = await _repository.GetTotalMessagesAsync();
            Assert.That(count, Is.EqualTo(12500));
        }
        
        [Test]
        public async Task GetTotalRoomsAsync_ReturnsCount()
        {
            var count = await _repository.GetTotalRoomsAsync();
            Assert.That(count, Is.EqualTo(25));
        }
    }
}