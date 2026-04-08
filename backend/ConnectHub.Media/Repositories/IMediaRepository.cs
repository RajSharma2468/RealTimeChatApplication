using ConnectHub.Media.Models;

namespace ConnectHub.Media.Repositories
{
    public interface IMediaRepository
    {
        Task<MediaFile> CreateAsync(MediaFile mediaFile);
        Task<MediaFile?> GetByIdAsync(int id);
        Task<IEnumerable<MediaFile>> GetByMessageIdAsync(int messageId);
        Task<IEnumerable<MediaFile>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<MediaFile>> GetByUserAsync(int userId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MediaFile>> GetExpiredFilesAsync();
        Task<bool> DeleteExpiredFilesAsync();
    }
}