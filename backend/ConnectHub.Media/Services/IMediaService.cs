using ConnectHub.Media.DTOs;

namespace ConnectHub.Media.Services
{
    public interface IMediaService
    {
        // Upload operations
        Task<UploadFileDto> UploadFileAsync(int userId, IFormFile file, int? messageId = null, int? roomId = null);
        
        // Get file operations (with authentication)
        Task<MediaResponseDto> GetFileByIdAsync(int fileId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetFilesByMessageAsync(int messageId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetFilesByRoomAsync(int roomId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetMyFilesAsync(int userId);
        
        // Delete operation
        Task<bool> DeleteFileAsync(int fileId, int userId);
        
        // Public download (no authentication required)
        Task<MediaResponseDto> GetFileByIdForDownloadAsync(int fileId);  
        
        // Cleanup expired files
        Task<bool> CleanupExpiredFilesAsync();
    }
}