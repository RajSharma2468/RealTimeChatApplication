using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using ConnectHub.Media.Data;
using ConnectHub.Media.Models;
using ConnectHub.Media.Repositories;

namespace ConnectHub.Media.Tests.UnitTests
{
    [TestFixture]
    public class MediaRepositoryTests
    {
        private MediaDbContext _context;
        private MediaRepository _repository;
        
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MediaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new MediaDbContext(options);
            _repository = new MediaRepository(_context);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
        
        [Test]
        public async Task CreateAsync_ValidFile_AddsToDatabase()
        {
            var file = new MediaFile
            {
                FileName = "test.jpg",
                FilePath = "/uploads/test.jpg",
                ContentType = "image/jpeg",
                FileSize = 1024,
                UploadedBy = 1
            };
            
            var result = await _repository.CreateAsync(file);
            
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.FileName, Is.EqualTo("test.jpg"));
        }
        
        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsFile()
        {
            var file = new MediaFile
            {
                FileName = "test.jpg",
                FilePath = "/uploads/test.jpg",
                ContentType = "image/jpeg",
                FileSize = 1024,
                UploadedBy = 1
            };
            var created = await _repository.CreateAsync(file);
            
            var result = await _repository.GetByIdAsync(created.Id);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(created.Id));
        }
        
        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _repository.GetByIdAsync(999);
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public async Task DeleteAsync_ExistingFile_ReturnsTrue()
        {
            var file = new MediaFile
            {
                FileName = "test.jpg",
                FilePath = "/uploads/test.jpg",
                ContentType = "image/jpeg",
                FileSize = 1024,
                UploadedBy = 1
            };
            var created = await _repository.CreateAsync(file);
            
            var result = await _repository.DeleteAsync(created.Id);
            
            Assert.That(result, Is.True);
        }
    }
}