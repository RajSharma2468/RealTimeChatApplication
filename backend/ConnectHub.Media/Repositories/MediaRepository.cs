using Microsoft.EntityFrameworkCore;
using ConnectHub.Media.Models;
using ConnectHub.Media.Data;

namespace ConnectHub.Media.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly MediaDbContext _context;
        
        public MediaRepository(MediaDbContext context)
        {
            _context = context;
        }
        
        public async Task<MediaFile> CreateAsync(MediaFile mediaFile)
        {
            _context.MediaFiles.Add(mediaFile);
            await _context.SaveChangesAsync();
            return mediaFile;
        }
        
        public async Task<MediaFile?> GetByIdAsync(int id)
        {
            return await _context.MediaFiles.FindAsync(id);
        }
        
        public async Task<IEnumerable<MediaFile>> GetByMessageIdAsync(int messageId)
        {
            return await _context.MediaFiles
                .Where(m => m.MessageId == messageId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<MediaFile>> GetByRoomIdAsync(int roomId)
        {
            return await _context.MediaFiles
                .Where(m => m.RoomId == roomId)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<MediaFile>> GetByUserAsync(int userId)
        {
            return await _context.MediaFiles
                .Where(m => m.UploadedBy == userId)
                .OrderByDescending(m => m.UploadedAt)
                .ToListAsync();
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var mediaFile = await GetByIdAsync(id);
            if (mediaFile == null) return false;
            
            _context.MediaFiles.Remove(mediaFile);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<MediaFile>> GetExpiredFilesAsync()
        {
            return await _context.MediaFiles
                .Where(m => m.ExpiresAt != null && m.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }
        
        public async Task<bool> DeleteExpiredFilesAsync()
        {
            var expiredFiles = await GetExpiredFilesAsync();
            if (!expiredFiles.Any()) return false;
            
            _context.MediaFiles.RemoveRange(expiredFiles);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}