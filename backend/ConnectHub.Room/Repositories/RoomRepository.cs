using ConnectHub.Room.DTOs;
using ConnectHub.Room.Models;
using ConnectHub.Room.Repositories;
using System.Text.Json;

namespace ConnectHub.Room.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public RoomService(IRoomRepository roomRepository, IHttpClientFactory httpClientFactory)
        {
            _roomRepository = roomRepository;
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<RoomResponseDto> CreateRoomAsync(int userId, CreateRoomDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoomName))
                throw new Exception("Room name is required");
            
            var room = new ChatRoom
            {
                RoomName = dto.RoomName,
                Description = dto.Description,
                RoomType = dto.RoomType,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            var createdRoom = await _roomRepository.CreateRoomAsync(room);
            
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
        
        public async Task<RoomResponseDto> GetRoomByIdAsync(int roomId, int currentUserId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");
            
            return await MapToResponseDto(room, currentUserId);
        }
        
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
        
        // ================================================================
        // JOIN ROOM - FIXED: Already member hai to reactivate karo
        // ================================================================
        public async Task<bool> JoinRoomAsync(int roomId, int userId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null)
                throw new Exception("Room not found");
            
            if (room.RoomType != "PUBLIC")
                throw new Exception("Cannot join private room");
            
            var existingMember = await _roomRepository.GetMemberAsync(roomId, userId);
            
            if (existingMember != null)
            {
                if (existingMember.IsActive)
                {
                    // Already active member - success return
                    return true;
                }
                else
                {
                    // Inactive member - reactivate using SQL
                    await _roomRepository.ExecuteReactivateAsync(roomId, userId);
                    return true;
                }
            }
            
            var memberCount = await _roomRepository.GetMemberCountAsync(roomId);
            if (memberCount >= room.MaxMembers)
                throw new Exception("Room is full");
            
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
        
        public async Task<bool> LeaveRoomAsync(int roomId, int userId)
        {
            var isMember = await _roomRepository.IsUserInRoomAsync(roomId, userId);
            if (!isMember)
                throw new Exception("You are not a member of this room");
            
            return await _roomRepository.RemoveMemberAsync(roomId, userId);
        }
        
        public async Task<bool> RemoveMemberAsync(int roomId, int userIdToRemove, int currentUserId)
        {
            var isAdmin = await IsUserAdminAsync(roomId, currentUserId);
            if (!isAdmin)
                throw new Exception("Only room admin can remove members");
            
            if (userIdToRemove == currentUserId)
                throw new Exception("Use 'Leave Room' to remove yourself");
            
            var isMember = await _roomRepository.IsUserInRoomAsync(roomId, userIdToRemove);
            if (!isMember)
                throw new Exception("User is not a member of this room");
            
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room != null && room.CreatedBy == userIdToRemove)
                throw new Exception("Cannot remove the room creator");
            
            return await _roomRepository.RemoveMemberAsync(roomId, userIdToRemove);
        }
        
        public async Task<bool> UpdateMemberRoleAsync(int adminUserId, UpdateMemberRoleDto dto)
        {
            var isAdmin = await IsUserAdminAsync(dto.RoomId, adminUserId);
            if (!isAdmin)
                throw new Exception("Only room admin can update member roles");
            
            var targetMember = await _roomRepository.GetMemberAsync(dto.RoomId, dto.UserId);
            if (targetMember == null)
                throw new Exception("User is not a member of this room");
            
            return await _roomRepository.UpdateMemberRoleAsync(dto.RoomId, dto.UserId, dto.NewRole);
        }
        
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
        
        public async Task<bool> IsUserAdminAsync(int roomId, int userId)
        {
            var member = await _roomRepository.GetMemberAsync(roomId, userId);
            return member != null && member.Role == "ADMIN";
        }
        
        public async Task<bool> IsUserInRoomAsync(int roomId, int userId)
        {
            return await _roomRepository.IsUserInRoomAsync(roomId, userId);
        }
        
        public async Task<RoomMember?> GetMemberAsync(int roomId, int userId)
        {
            return await _roomRepository.GetMemberAsync(roomId, userId);
        }
        
        public async Task<IEnumerable<RoomMemberDto>> GetRoomMembersAsync(int roomId, int currentUserId, string token)
        {
            var isMember = await _roomRepository.IsUserInRoomAsync(roomId, currentUserId);
            if (!isMember)
                throw new Exception("You are not a member of this room");
            
            var members = await _roomRepository.GetRoomMembersAsync(roomId);
            var result = new List<RoomMemberDto>();
            
            using var httpClient = _httpClientFactory.CreateClient();
            
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            }
            
            foreach (var member in members)
            {
                string userName = $"User_{member.UserId}";
                
                try
                {
                    var response = await httpClient.GetAsync($"https://connecthub-auth-brdsdmghhhgwaphq.centralus-01.azurewebsites.net/api/auth/{member.UserId}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var userData = JsonSerializer.Deserialize<AuthUserResponse>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (userData?.Success == true && userData.Data != null)
                        {
                            userName = userData.Data.DisplayName ?? userData.Data.Username ?? $"User_{member.UserId}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch user {member.UserId}: {ex.Message}");
                }
                
                result.Add(new RoomMemberDto
                {
                    UserId = member.UserId,
                    UserName = userName,
                    Role = member.Role,
                    JoinedAt = member.JoinedAt
                });
            }
            
            return result;
        }
        
        private async Task<string> GetUserRoleAsync(int roomId, int userId)
        {
            var member = await _roomRepository.GetMemberAsync(roomId, userId);
            return member?.Role ?? "NONE";
        }
        
        private async Task<RoomResponseDto> MapToResponseDto(ChatRoom room, int currentUserId)
        {
            var memberCount = await _roomRepository.GetMemberCountAsync(room.Id);
            var userRole = await GetUserRoleAsync(room.Id, currentUserId);
            
            string creatorName = $"User_{room.CreatedBy}";
            
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://connecthub-auth-brdsdmghhhgwaphq.centralus-01.azurewebsites.net/api/auth/{room.CreatedBy}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var userData = JsonSerializer.Deserialize<AuthUserResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (userData?.Success == true && userData.Data != null)
                    {
                        creatorName = userData.Data.DisplayName ?? userData.Data.Username ?? creatorName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch creator {room.CreatedBy}: {ex.Message}");
            }
            
            return new RoomResponseDto
            {
                Id = room.Id,
                RoomName = room.RoomName,
                Description = room.Description,
                RoomType = room.RoomType,
                AvatarUrl = room.AvatarUrl,
                CreatedBy = room.CreatedBy,
                CreatorName = creatorName,
                CreatedAt = room.CreatedAt,
                MemberCount = memberCount,
                UserRole = userRole
            };
        }
    }
    
    public class AuthUserResponse
    {
        public bool Success { get; set; }
        public AuthUserData Data { get; set; }
    }
    
    public class AuthUserData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}