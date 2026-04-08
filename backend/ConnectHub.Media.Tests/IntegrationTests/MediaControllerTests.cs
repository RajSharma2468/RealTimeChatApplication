using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using ConnectHub.Media.Controllers;
using ConnectHub.Media.Services;
using ConnectHub.Media.DTOs;

namespace ConnectHub.Media.Tests.IntegrationTests
{
    [TestFixture]
    public class MediaControllerTests
    {
        private Mock<IMediaService> _mockService;
        private MediaController _controller;
        
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IMediaService>();
            _controller = new MediaController(_mockService.Object);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        [Test]
        public async Task UploadFile_NoFile_ReturnsBadRequest()
        {
            var result = await _controller.UploadFile(null, null, null);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
        
        [Test]
        public async Task GetFile_ExistingId_ReturnsOk()
        {
            var fileDto = new MediaResponseDto 
            { 
                Id = 1, 
                FileName = "test.jpg", 
                FileUrl = "/uploads/test.jpg", 
                ContentType = "image/jpeg" 
            };
            _mockService.Setup(x => x.GetFileByIdAsync(1, 1)).ReturnsAsync(fileDto);
            
            var result = await _controller.GetFile(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task GetFile_NonExistingId_ReturnsNotFound()
        {
            _mockService.Setup(x => x.GetFileByIdAsync(999, 1))
                .ThrowsAsync(new Exception("File not found"));
            
            var result = await _controller.GetFile(999);
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
        
        [Test]
        public async Task GetMyFiles_ReturnsOk()
        {
            var files = new List<MediaResponseDto>();
            _mockService.Setup(x => x.GetMyFilesAsync(1)).ReturnsAsync(files);
            
            var result = await _controller.GetMyFiles();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteFile_ValidId_ReturnsOk()
        {
            _mockService.Setup(x => x.DeleteFileAsync(1, 1)).ReturnsAsync(true);
            
            var result = await _controller.DeleteFile(1);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        
        [Test]
        public async Task DeleteFile_InvalidId_ReturnsBadRequest()
        {
            _mockService.Setup(x => x.DeleteFileAsync(999, 1))
                .ThrowsAsync(new Exception("File not found"));
            
            var result = await _controller.DeleteFile(999);
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}