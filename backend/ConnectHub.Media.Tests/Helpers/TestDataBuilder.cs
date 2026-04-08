using ConnectHub.Media.Models;
using ConnectHub.Media.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace ConnectHub.Media.Tests.Helpers
{
    // Creates test data for media tests
    public static class TestDataBuilder
    {
        // Create a test media file
        public static MediaFile CreateTestMediaFile(
            int id = 0,
            int uploadedBy = 1,
            string fileName = "test.jpg",
            string contentType = "image/jpeg",
            long fileSize = 1024,
            int? messageId = null,
            int? roomId = null)
        {
            return new MediaFile
            {
                Id = id,
                FileName = fileName,
                FilePath = $"/uploads/{Guid.NewGuid()}_{fileName}",
                ContentType = contentType,
                FileSize = fileSize,
                UploadedBy = uploadedBy,
                MessageId = messageId,
                RoomId = roomId,
                UploadedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
        }
        
        // Create expired media file (for cleanup testing)
        public static MediaFile CreateExpiredMediaFile(int id = 99, int uploadedBy = 1)
        {
            var file = CreateTestMediaFile(id, uploadedBy);
            file.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            return file;
        }
        
        // Create upload response DTO
        public static UploadFileDto CreateUploadFileDto(int fileId = 1, string fileName = "test.jpg")
        {
            return new UploadFileDto
            {
                FileId = fileId,
                FileName = fileName,
                FileUrl = $"/uploads/{fileName}",
                FileSize = 1024,
                ContentType = "image/jpeg",
                UploadedAt = DateTime.UtcNow
            };
        }
        
        // Create multiple test files for a user
        public static List<MediaFile> CreateMultipleTestFiles(int userId, int count = 5)
        {
            var files = new List<MediaFile>();
            for (int i = 1; i <= count; i++)
            {
                files.Add(CreateTestMediaFile(id: i, uploadedBy: userId, fileName: $"file{i}.jpg"));
            }
            return files;
        }
        
        // Create mock IFormFile for testing
        public static IFormFile CreateMockFormFile(string fileName = "test.jpg", string contentType = "image/jpeg", long fileSize = 1024)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(new string('A', (int)fileSize));
            writer.Flush();
            stream.Position = 0;
            
            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
        
        // Create large file (exceeds limit)
        public static IFormFile CreateLargeMockFormFile()
        {
            return CreateMockFormFile("large.jpg", "image/jpeg", 15 * 1024 * 1024);
        }
        
        // Create media response DTO
        public static MediaResponseDto CreateMediaResponseDto(MediaFile file)
        {
            return new MediaResponseDto
            {
                Id = file.Id,
                FileName = file.FileName,
                FileUrl = file.FilePath,
                ContentType = file.ContentType,
                FileSize = file.FileSize,
                UploadedBy = file.UploadedBy,
                UploadedByName = $"User_{file.UploadedBy}",
                UploadedAt = file.UploadedAt,
                MessageId = file.MessageId,
                RoomId = file.RoomId,
                ExpiresAt = file.ExpiresAt
            };
        }
    }
}