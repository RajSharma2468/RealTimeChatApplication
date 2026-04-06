using ConnectHub.Room.DTOs;
using ConnectHub.Room.Models;
using ConnectHub.Room.Repositories;

namespace ConnectHub.Room.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        
        // Dependency Injection: Repository injected
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        
        // Create a new room - User becomes ADMIN automatically
        public async Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto)
        {
            // Validate room name
            if (string.IsNullOrWhiteSpace(dto.RoomName))
                throw new Exception("Room name is required");
            
            // Create room entity
            var room = new ChatRoom
            {
                RoomName = dto.RoomName,
                Description = dto.Description,
                RoomType = dto.RoomType,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            // Save room to database
            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            
            // Add creator as ADMIN member
            var member = new RoomMember
            {
                RoomId = createdRoom.Id,
                UserId = userId,
                Role = "ADMIN",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _roomRepository.AddMemberAsync(member);
            
            return await MapToResponseDto(createdRoom, userId);
        }
        
        // Get room by ID with member info
        public async Task<RoomResponseDto> GetRoomByIdAsync(int roomId, int currentUserId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");
            
            return await MapToResponseDto(room, currentUserId);
        }
        
        // Get all public rooms
        public async Task<IEnumerable<RoomListDto>> GetPublicRoomsAsync(int currentUserId)
        {
            var rooms = await _roomRepository.GetPublicRoomsAsync();
            var result = new List<RoomListDto>();
            
            foreach (var room in rooms)
            {
                var memberCount = await _roomRepository.GetMemberCountAsync(room.Id);
                var isMember = await _roomRepository.IsUserInRoomAsync(room.Id, currentUserId);
                
                result.Add(new RoomListDto
                {
                    Id = room.Id,
                    RoomName = room.RoomName,
                    Description = room.Description,
                    RoomType = room.RoomType,
                    MemberCount = memberCount,
                    IsMember = isMember
                });
            }
            
            return result;
        }
        
        // Get rooms where current user is a member
        public async Task<IEnumerable<RoomListDto>> GetMyRoomsAsync(int userId)
        {
            var rooms = await _roomRepository.GetRoomsByUserIdAsync(userId);
            var result = new List<RoomListDto>();
            
            foreach (var room in rooms)
            {
                var memberCount = await _roomRepository.GetMemberCountAsync(room.Id);
                var userRole = await GetUserRoleAsync(room.Id, userId);
                
                result.Add(new RoomListDto
                {
                    Id = room.Id,
                    RoomName = room.RoomName,
                    Description = room.Description,
                    RoomType = room.RoomType,
                    MemberCount = memberCount,
                    IsMember = true,
                    UserRole = userRole
                });
            }
            
            return result;
        }
        
        // Join a public room
        public async Task<bool> JoinRoomAsync(int roomId, int userId)
        {
            // Check if room exists and is public
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");
            
            if (room.RoomType != "PUBLIC")
                throw new Exception("Cannot join private room");
            
            // Check if already a member
            var isMember = await _roomRepository.IsUserInRoomAsync(roomId, userId);
            if (isMember)
                throw new Exception("Already a member of this room");
            
            // Check member limit
            var memberCount = await _roomRepository.GetMemberCountAsync(roomId);
            if (memberCount >= room.MaxMembers)
                throw new Exception("Room is full");
            
            // Add member
            var member = new RoomMember
            {
                RoomId = roomId,
                UserId = userId,
                Role = "MEMBER",
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            await _roomRepository.AddMemberAsync(member);
            return true;
        }
        
        // Leave a room
        public async Task<bool> LeaveRoomAsync(int roomId, int userId)
        {
            // Check if member exists
            var isMember = await _roomRepository.IsUserInRoomAsync(roomId, userId);
            if (!isMember)
                throw new Exception("You are not a member of this room");
            
            // Get member to check if ADMIN (creator cannot leave? optional)
            var member = await _roomRepository.GetMemberAsync(roomId, userId);
            if (member != null && member.Role == "ADMIN")
            {
                // Optional: Prevent admin from leaving or transfer ownership
                // For now, allow leaving
            }
            
            return await _roomRepository.RemoveMemberAsync(roomId, userId);
        }
        
        // Update member role - Only ADMIN can do this
        public async Task<bool> UpdateMemberRoleAsync(int adminUserId, UpdateMemberRoleDto dto)
        {
            // Verify admin is actually ADMIN of this room
            var isAdmin = await IsUserAdminAsync(dto.RoomId, adminUserId);
            if (!isAdmin)
                throw new Exception("Only room admin can update member roles");
            
            // Cannot change role of another admin? (optional)
            var targetMember = await _roomRepository.GetMemberAsync(dto.RoomId, dto.UserId);
            if (targetMember == null)
                throw new Exception("User is not a member of this room");
            
            // Update role
            return await _roomRepository.UpdateMemberRoleAsync(dto.RoomId, dto.UserId, dto.NewRole);
        }
        
        // Delete room - Only creator or admin can delete
        public async Task<bool> DeleteRoomAsync(int roomId, int userId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");
            
            if (room.CreatedBy != userId)
            {
                var isAdmin = await IsUserAdminAsync(roomId, userId);
                if (!isAdmin)
                    throw new Exception("Only room creator or admin can delete the room");
            }
            
            return await _roomRepository.DeleteRoomAsync(roomId);
        }
        
        // Check if user is admin of a room
        public async Task<bool> IsUserAdminAsync(int roomId, int userId)
        {
            var member = await _roomRepository.GetMemberAsync(roomId, userId);
            return member != null && member.Role == "ADMIN";
        }
        
        // Helper: Get user role in room
        private async Task<string> GetUserRoleAsync(int roomId, int userId)
        {
            var member = await _roomRepository.GetMemberAsync(roomId, userId);
            return member?.Role ?? "NONE";
        }
        
        // Helper: Map ChatRoom entity to Response DTO
        private async Task<RoomResponseDto> MapToResponseDto(ChatRoom room, int currentUserId)
        {
            var memberCount = await _roomRepository.GetMemberCountAsync(room.Id);
            var userRole = await GetUserRoleAsync(room.Id, currentUserId);
            
            return new RoomResponseDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                Description = room.Description,
                RoomType = room.RoomType,
                AvatarUrl = room.AvatarUrl,
                CreatedBy = room.CreatedBy,
                CreatorName = $"User_{room.CreatedBy}",  // Will be enhanced with Auth service
                CreatedAt = room.CreatedAt,
                MemberCount = memberCount,
                UserRole = userRole
            };
        }
    }
}