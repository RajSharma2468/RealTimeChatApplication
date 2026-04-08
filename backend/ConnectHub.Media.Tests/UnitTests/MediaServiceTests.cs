using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Hosting;
using ConnectHub.Media.Services;
using ConnectHub.Media.Repositories;
using ConnectHub.Media.Models;

namespace ConnectHub.Media.Tests.UnitTests
{
    [TestFixture]
    public class MediaServiceTests
    {
        private Mock<IMediaRepository> _mockRepo;
        private Mock<IWebHostEnvironment> _mockEnvironment;
        private MediaService _service;
        
        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IMediaRepository>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());
            _service = new MediaService(_mockRepo.Object, _mockEnvironment.Object);
        }
        
        [Test]
        public async Task GetFileByIdAsync_ExistingFile_ReturnsFileDto()
        {
            // Arrange
            var file = new MediaFile 
            { 
                Id = 1, 
                FileName = "test.jpg", 
                FilePath = "/uploads/test.jpg", 
                ContentType = "image/jpeg", 
                FileSize = 1024, 
                UploadedBy = 1 
            };
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(file);
            
            // Act
            var result = await _service.GetFileByIdAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }
        
        [Test]
        public void GetFileByIdAsync_NonExistingFile_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((MediaFile?)null);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.GetFileByIdAsync(999, 1));
            Assert.That(ex.Message, Is.EqualTo("File not found"));
        }
        
        [Test]
        public async Task DeleteFileAsync_AsOwner_DeletesFile()
        {
            // Arrange - Mock file with a non-existent path so file deletion doesn't fail
            var file = new MediaFile 
            { 
                Id = 1, 
                UploadedBy = 1,
                FilePath = "/uploads/nonexistent.jpg"  // File doesn't exist, deletion will succeed
            };
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(file);
            _mockRepo.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);
            
            // Act
            var result = await _service.DeleteFileAsync(1, 1);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void DeleteFileAsync_AsNonOwner_ThrowsException()
        {
            // Arrange
            var file = new MediaFile { Id = 1, UploadedBy = 2 };
            _mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(file);
            
            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.DeleteFileAsync(1, 1));
            Assert.That(ex.Message, Is.EqualTo("You can only delete your own files"));
        }
        
        [Test]
        public async Task GetMyFilesAsync_ReturnsUserFiles()
        {
            // Arrange
            var files = new List<MediaFile>
            {
                new MediaFile { Id = 1, UploadedBy = 1, FileName = "file1.jpg" },
                new MediaFile { Id = 2, UploadedBy = 1, FileName = "file2.jpg" }
            };
            _mockRepo.Setup(x => x.GetByUserAsync(1)).ReturnsAsync(files);
            
            // Act
            var result = await _service.GetMyFilesAsync(1);
            
            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}