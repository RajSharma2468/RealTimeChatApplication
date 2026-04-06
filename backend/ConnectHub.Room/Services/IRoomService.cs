using ConnectHub.Room.DTOs;

namespace ConnectHub.Room.Services
{
    // Service interface - contains business logic
    public interface IRoomService
    {
        // Room operations
        Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto);
        Task<RoomResponseDto> GetRoomByIdAsync(int roomId, int currentUserId);
        Task<IEnumerable<RoomListDto>> GetPublicRoomsAsync(int currentUserId);
        Task<IEnumerable<RoomListDto>> GetMyRoomsAsync(int userId);
        Task<bool> DeleteRoomAsync(int roomId, int userId);
        
        // Member operations
        Task<bool> JoinRoomAsync(int roomId, int userId);
        Task<bool> LeaveRoomAsync(int roomId, int userId);
        Task<bool> UpdateMemberRoleAsync(int adminUserId, UpdateMemberRoleDto dto);
        Task<bool> IsUserAdminAsync(int roomId, int userId);
    }
}