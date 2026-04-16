using ConnectHub.Room.Models;

namespace ConnectHub.Room.Repositories
{
    public interface IRoomRepository
    {
        // ================================================================
        // ROOM OPERATIONS
        // ================================================================
        Task<ChatRoom> CreateRoomAsync(ChatRoom room);
        Task<ChatRoom?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<ChatRoom>> GetPublicRoomsAsync();
        Task<IEnumerable<ChatRoom>> GetRoomsByUserIdAsync(int userId);
        Task<ChatRoom> UpdateRoomAsync(ChatRoom room);
        Task<bool> DeleteRoomAsync(int roomId);
        
        // ================================================================
        // MEMBER OPERATIONS
        // ================================================================
        Task<RoomMember> AddMemberAsync(RoomMember member);
        Task<RoomMember?> GetMemberAsync(int roomId, int userId);
        Task<bool> UpdateMemberAsync(RoomMember member);  // ADD THIS
        Task<bool> ReactivateMemberAsync(int roomId, int userId);  // ADD THIS
        Task<bool> RemoveMemberAsync(int roomId, int userId);
        Task<IEnumerable<RoomMember>> GetRoomMembersAsync(int roomId);
        Task<bool> IsUserInRoomAsync(int roomId, int userId);
        Task<int> GetMemberCountAsync(int roomId);
        Task<bool> UpdateMemberRoleAsync(int roomId, int userId, string newRole);
        
        // ================================================================
        // UTILITY
        // ================================================================
        Task<bool> RoomExistsAsync(int roomId);
    }
}