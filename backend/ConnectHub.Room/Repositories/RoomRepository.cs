using Microsoft.EntityFrameworkCore;
using ConnectHub.Room.Models;
using ConnectHub.Room.Data;

namespace ConnectHub.Room.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomDbContext _context;
        
        // Dependency Injection: DbContext injected by framework
        public RoomRepository(RoomDbContext context)
        {
            _context = context;
        }
        
        // Create a new room
        public async Task<ChatRoom> CreateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        // Get room by ID (includes soft-deleted filter automatically)
        public async Task<ChatRoom?> GetRoomByIdAsync(int roomId)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        
        // Get all public rooms
        public async Task<IEnumerable<ChatRoom>> GetPublicRoomsAsync()
        {
            return await _context.ChatRooms
                .Where(r => r.RoomType == "PUBLIC")
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        
        // Get rooms where user is a member
        public async Task<IEnumerable<ChatRoom>> GetRoomsByUserIdAsync(int userId)
        {
            var roomIds = await _context.RoomMembers
                .Where(rm => rm.UserId == userId)
                .Select(rm => rm.RoomId)
                .ToListAsync();
            
            return await _context.ChatRooms
                .Where(r => roomIds.Contains(r.Id))
                .ToListAsync();
        }
        
        // Update room details
        public async Task<ChatRoom> UpdateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        // Soft delete room
        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var room = await GetRoomByIdAsync(roomId);
            if (room == null) return false;
            
            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Add member to room
        public async Task<RoomMember> AddMemberAsync(RoomMember member)
        {
            _context.RoomMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }
        
        // Get member details
        public async Task<RoomMember?> GetMemberAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
        }
        
        // Remove member from room
        public async Task<bool> RemoveMemberAsync(int roomId, int userId)
        {
            var member = await GetMemberAsync(roomId, userId);
            if (member == null) return false;
            
            member.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Get all members of a room
        public async Task<IEnumerable<RoomMember>> GetRoomMembersAsync(int roomId)
        {
            return await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId)
                .ToListAsync();
        }
        
        // Check if user is in room
        public async Task<bool> IsUserInRoomAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .AnyAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
        }
        
        // Get member count
        public async Task<int> GetMemberCountAsync(int roomId)
        {
            return await _context.RoomMembers
                .CountAsync(rm => rm.RoomId == roomId);
        }
        
        // Update member role
        public async Task<bool> UpdateMemberRoleAsync(int roomId, int userId, string newRole)
        {
            var member = await GetMemberAsync(roomId, userId);
            if (member == null) return false;
            
            member.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Check if room exists
        public async Task<bool> RoomExistsAsync(int roomId)
        {
            return await _context.ChatRooms.AnyAsync(r => r.Id == roomId);
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}