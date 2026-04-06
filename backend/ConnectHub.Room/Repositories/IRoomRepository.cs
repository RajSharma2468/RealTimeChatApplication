using ConnectHub.Room.Models;

namespace ConnectHub.Room.Repositories
{
    // Repository interface - defines database operations
    // WHY: Abstraction, testability, loose coupling
    public interface IRoomRepository
    {
        // Room operations
        Task<ChatRoom> CreateRoomAsync(ChatRoom room);
        Task<ChatRoom?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<ChatRoom>> GetPublicRoomsAsync();
        Task<IEnumerable<ChatRoom>> GetRoomsByUserIdAsync(int userId);
        Task<ChatRoom> UpdateRoomAsync(ChatRoom room);
        Task<bool> DeleteRoomAsync(int roomId);
        
        // Member operations
        Task<RoomMember> AddMemberAsync(RoomMember member);
        Task<RoomMember?> GetMemberAsync(int roomId, int userId);
        Task<bool> RemoveMemberAsync(int roomId, int userId);
        Task<IEnumerable<RoomMember>> GetRoomMembersAsync(int roomId);
        Task<bool> IsUserInRoomAsync(int roomId, int userId);
        Task<int> GetMemberCountAsync(int roomId);
        Task<bool> UpdateMemberRoleAsync(int roomId, int userId, string newRole);
        
        // Utility
        Task<bool> RoomExistsAsync(int roomId);
        Task<int> SaveChangesAsync();
    }
}