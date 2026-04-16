using Microsoft.EntityFrameworkCore;
using ConnectHub.Room.Models;
using ConnectHub.Room.Data;

namespace ConnectHub.Room.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly RoomDbContext _context;
        
        public RoomRepository(RoomDbContext context)
        {
            _context = context;
        }
        
        // ================================================================
        // ROOM OPERATIONS
        // ================================================================
        
        public async Task<ChatRoom> CreateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        public async Task<ChatRoom?> GetRoomByIdAsync(int roomId)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        
        public async Task<IEnumerable<ChatRoom>> GetPublicRoomsAsync()
        {
            return await _context.ChatRooms
                .Where(r => r.RoomType == "PUBLIC" && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<ChatRoom>> GetRoomsByUserIdAsync(int userId)
        {
            var roomIds = await _context.RoomMembers
                .Where(rm => rm.UserId == userId && rm.IsActive)
                .Select(rm => rm.RoomId)
                .ToListAsync();
            
            return await _context.ChatRooms
                .Where(r => roomIds.Contains(r.Id) && r.IsActive)
                .ToListAsync();
        }
        
        public async Task<ChatRoom> UpdateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            var room = await GetRoomByIdAsync(roomId);
            if (room == null) return false;
            
            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // ================================================================
        // MEMBER OPERATIONS
        // ================================================================
        
        public async Task<RoomMember> AddMemberAsync(RoomMember member)
        {
            _context.RoomMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }
        
        public async Task<RoomMember?> GetMemberAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
        }
        
        // ================================================================
        // UPDATE MEMBER - Add this method
        // ================================================================
        public async Task<bool> UpdateMemberAsync(RoomMember member)
        {
            _context.RoomMembers.Update(member);
            await _context.SaveChangesAsync();
            return true;
        }
        
        // ================================================================
        // REACTIVATE MEMBER - Direct SQL for reactivation
        // ================================================================
        public async Task<bool> ReactivateMemberAsync(int roomId, int userId)
        {
            try
            {
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE \"RoomMembers\" SET \"IsActive\" = true, \"JoinedAt\" = NOW() WHERE \"RoomId\" = {0} AND \"UserId\" = {1}",
                    roomId, userId);
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReactivateMemberAsync Error: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> RemoveMemberAsync(int roomId, int userId)
        {
            var member = await GetMemberAsync(roomId, userId);
            if (member == null) return false;
            
            member.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<RoomMember>> GetRoomMembersAsync(int roomId)
        {
            return await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId && rm.IsActive)
                .ToListAsync();
        }
        
        public async Task<bool> IsUserInRoomAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .AnyAsync(rm => rm.RoomId == roomId && rm.UserId == userId && rm.IsActive);
        }
        
        public async Task<int> GetMemberCountAsync(int roomId)
        {
            return await _context.RoomMembers
                .CountAsync(rm => rm.RoomId == roomId && rm.IsActive);
        }
        
        public async Task<bool> UpdateMemberRoleAsync(int roomId, int userId, string newRole)
        {
            var member = await GetMemberAsync(roomId, userId);
            if (member == null) return false;
            
            member.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // ================================================================
        // UTILITY
        // ================================================================
        
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