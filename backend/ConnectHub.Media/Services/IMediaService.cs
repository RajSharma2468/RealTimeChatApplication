using ConnectHub.Media.DTOs;

namespace ConnectHub.Media.Services
{
    public interface IMediaService
    {
        Task<UploadFileDto> UploadFileAsync(int userId, IFormFile file, int? messageId = null, int? roomId = null);
        Task<MediaResponseDto> GetFileByIdAsync(int fileId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetFilesByMessageAsync(int messageId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetFilesByRoomAsync(int roomId, int userId);
        Task<IEnumerable<MediaResponseDto>> GetMyFilesAsync(int userId);
        Task<bool> DeleteFileAsync(int fileId, int userId);
        Task<bool> CleanupExpiredFilesAsync();
    }
}