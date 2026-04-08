using ConnectHub.Media.Models;
using ConnectHub.Media.Repositories;

namespace ConnectHub.Media.Tests.Mocks
{
    public class MockMediaRepository : IMediaRepository
    {
        private readonly List<MediaFile> _files = new();
        private int _nextId = 1;
        
        public Task<MediaFile> CreateAsync(MediaFile mediaFile)
        {
            mediaFile.Id = _nextId++;
            _files.Add(mediaFile);
            return Task.FromResult(mediaFile);
        }
        
        public Task<MediaFile?> GetByIdAsync(int id)
        {
            var file = _files.FirstOrDefault(f => f.Id == id);
            return Task.FromResult(file);
        }
        
        public Task<IEnumerable<MediaFile>> GetByMessageIdAsync(int messageId)
        {
            var result = _files.Where(f => f.MessageId == messageId);
            return Task.FromResult(result);
        }
        
        public Task<IEnumerable<MediaFile>> GetByRoomIdAsync(int roomId)
        {
            var result = _files.Where(f => f.RoomId == roomId);
            return Task.FromResult(result);
        }
        
        public Task<IEnumerable<MediaFile>> GetByUserAsync(int userId)
        {
            var result = _files.Where(f => f.UploadedBy == userId);
            return Task.FromResult(result);
        }
        
        public Task<bool> DeleteAsync(int id)
        {
            var file = _files.FirstOrDefault(f => f.Id == id);
            if (file == null) return Task.FromResult(false);
            _files.Remove(file);
            return Task.FromResult(true);
        }
        
        public Task<IEnumerable<MediaFile>> GetExpiredFilesAsync()
        {
            var result = _files.Where(f => f.ExpiresAt.HasValue && f.ExpiresAt < DateTime.UtcNow);
            return Task.FromResult(result);
        }
        
        public Task<bool> DeleteExpiredFilesAsync()
        {
            var expired = _files.Where(f => f.ExpiresAt.HasValue && f.ExpiresAt < DateTime.UtcNow).ToList();
            foreach (var file in expired)
            {
                _files.Remove(file);
            }
            return Task.FromResult(true);
        }
        
        // Helper method for tests
        public void AddTestFile(MediaFile file)
        {
            file.Id = _nextId++;
            _files.Add(file);
        }
        
        // Helper method to reset state
        public void Reset()
        {
            _files.Clear();
            _nextId = 1;
        }
    }
}