using ConnectHub.Room.DTOs;
using ConnectHub.Room.Models;

namespace ConnectHub.Room.Services
{
    public interface IRoomService
    {
        // ================================================================
        // ROOM OPERATIONS
        // ================================================================
        Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto);
        Task<RoomResponseDto> GetRoomByIdAsync(int roomId, int currentUserId);
        Task<IEnumerable<RoomListDto>> GetPublicRoomsAsync(int currentUserId);
        Task<IEnumerable<RoomListDto>> GetMyRoomsAsync(int userId);
        Task<bool> DeleteRoomAsync(int roomId, int userId);
        
        // ================================================================
        // MEMBER OPERATIONS
        // ================================================================
        Task<bool> JoinRoomAsync(int roomId, int userId);
        Task<bool> LeaveRoomAsync(int roomId, int userId);
        Task<bool> RemoveMemberAsync(int roomId, int userIdToRemove, int currentUserId);
        Task<bool> UpdateMemberRoleAsync(int adminUserId, UpdateMemberRoleDto dto);
        Task<bool> IsUserAdminAsync(int roomId, int userId);
        Task<bool> IsUserInRoomAsync(int roomId, int userId);
        Task<RoomMember?> GetMemberAsync(int roomId, int userId);
        
        // ================================================================
        // GET ROOM MEMBERS - WITH REAL NAMES
        // ================================================================
        Task<IEnumerable<RoomMemberDto>> GetRoomMembersAsync(int roomId, int currentUserId, string token);
    }
}