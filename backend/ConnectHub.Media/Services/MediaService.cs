using ConnectHub.Media.DTOs;
using ConnectHub.Media.Models;
using ConnectHub.Media.Repositories;

namespace ConnectHub.Media.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _mediaRepository;
        private readonly IWebHostEnvironment _environment;
        
        public MediaService(IMediaRepository mediaRepository, IWebHostEnvironment environment)
        {
            _mediaRepository = mediaRepository;
            _environment = environment;
        }
        
        // Upload file to server storage
        public async Task<UploadFileDto> UploadFileAsync(int userId, IFormFile file, int? messageId = null, int? roomId = null)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file provided");
            
            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                throw new Exception("File size exceeds 10MB limit");
            
            // Create uploads directory if not exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            
            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Save file metadata to database
            var mediaFile = new MediaFile
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{uniqueFileName}",
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedBy = userId,
                MessageId = messageId,
                RoomId = roomId,
                UploadedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)  // Expires in 30 days
            };
            
            var saved = await _mediaRepository.CreateAsync(mediaFile);
            
            return new UploadFileDto
            {
                FileId = saved.Id,
                FileName = saved.FileName,
                FileUrl = saved.FilePath,
                FileSize = saved.FileSize,
                ContentType = saved.ContentType,
                UploadedAt = saved.UploadedAt
            };
        }
        
        // Get file by ID
        public async Task<MediaResponseDto> GetFileByIdAsync(int fileId, int userId)
        {
            var file = await _mediaRepository.GetByIdAsync(fileId);
            if (file == null)
                throw new Exception("File not found");
            
            return MapToResponseDto(file);
        }
        
        // Get all files for a message
        public async Task<IEnumerable<MediaResponseDto>> GetFilesByMessageAsync(int messageId, int userId)
        {
            var files = await _mediaRepository.GetByMessageIdAsync(messageId);
            return files.Select(MapToResponseDto);
        }
        
        // Get all files for a room
        public async Task<IEnumerable<MediaResponseDto>> GetFilesByRoomAsync(int roomId, int userId)
        {
            var files = await _mediaRepository.GetByRoomIdAsync(roomId);
            return files.Select(MapToResponseDto);
        }
        
        // Get files uploaded by current user
        public async Task<IEnumerable<MediaResponseDto>> GetMyFilesAsync(int userId)
        {
            var files = await _mediaRepository.GetByUserAsync(userId);
            return files.Select(MapToResponseDto);
        }
        
        // Delete file (only by uploader)
        public async Task<bool> DeleteFileAsync(int fileId, int userId)
        {
            var file = await _mediaRepository.GetByIdAsync(fileId);
            if (file == null)
                throw new Exception("File not found");
            
            if (file.UploadedBy != userId)
                throw new Exception("You can only delete your own files");
            
            // Delete physical file
            var physicalPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", file.FilePath.TrimStart('/'));
            if (File.Exists(physicalPath))
                File.Delete(physicalPath);
            
            // Delete database record
            return await _mediaRepository.DeleteAsync(fileId);
        }
        
        // Background job: Clean up expired files
        public async Task<bool> CleanupExpiredFilesAsync()
        {
            var expiredFiles = await _mediaRepository.GetExpiredFilesAsync();
            
            foreach (var file in expiredFiles)
            {
                // Delete physical file
                var physicalPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", file.FilePath.TrimStart('/'));
                if (File.Exists(physicalPath))
                    File.Delete(physicalPath);
            }
            
            return await _mediaRepository.DeleteExpiredFilesAsync();
        }
        
        private MediaResponseDto MapToResponseDto(MediaFile file)
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