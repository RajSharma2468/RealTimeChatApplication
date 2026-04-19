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
        
        // Creates a new chat room in the database
        public async Task<ChatRoom> CreateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        // Fetches a single room by its ID
        public async Task<ChatRoom?> GetRoomByIdAsync(int roomId)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
        
        // Returns all active public rooms, newest first
        public async Task<IEnumerable<ChatRoom>> GetPublicRoomsAsync()
        {
            return await _context.ChatRooms
                .Where(r => r.RoomType == "PUBLIC" && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        
        // Returns all active rooms where the given user is a member
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
        
        // Updates an existing room's details
        public async Task<ChatRoom> UpdateRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }
        
        // Soft deletes a room by setting IsActive = false
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
        
        // Adds a new member to a room
        public async Task<RoomMember> AddMemberAsync(RoomMember member)
        {
            _context.RoomMembers.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }
        
        // Fetches a member record by roomId and userId
        // Uses AsNoTracking to avoid EF tracking conflicts during rejoins
        public async Task<RoomMember?> GetMemberAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
        }
        
        // Updates an existing member record
        public async Task<bool> UpdateMemberAsync(RoomMember member)
        {
            _context.RoomMembers.Update(member);
            await _context.SaveChangesAsync();
            return true;
        }
        
        // ================================================================
        // REACTIVATE MEMBER - EF Tracking approach
        // ================================================================
        public async Task<bool> ReactivateMemberAsync(int roomId, int userId)
        {
            try
            {
                var member = await _context.RoomMembers
                    .AsTracking()
                    .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
                
                if (member == null) return false;
                
                member.IsActive = true;
                member.JoinedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ReactivateMemberAsync Error: {ex.Message}");
                return false;
            }
        }
        
        public async Task ExecuteReactivateAsync(int roomId, int userId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"RoomMembers\" SET \"IsActive\" = true, \"JoinedAt\" = NOW() WHERE \"RoomId\" = {roomId} AND \"UserId\" = {userId}");
        }
        
        // Soft removes a member from a room by setting IsActive = false
        public async Task<bool> RemoveMemberAsync(int roomId, int userId)
        {
            var member = await _context.RoomMembers
                .AsTracking()
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
            if (member == null) return false;
            
            member.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Returns all active members of a room
        public async Task<IEnumerable<RoomMember>> GetRoomMembersAsync(int roomId)
        {
            return await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId && rm.IsActive)
                .ToListAsync();
        }
        
        // Checks if a user is an active member of a room
        public async Task<bool> IsUserInRoomAsync(int roomId, int userId)
        {
            return await _context.RoomMembers
                .AnyAsync(rm => rm.RoomId == roomId && rm.UserId == userId && rm.IsActive);
        }
        
        // Returns the count of active members in a room
        public async Task<int> GetMemberCountAsync(int roomId)
        {
            return await _context.RoomMembers
                .CountAsync(rm => rm.RoomId == roomId && rm.IsActive);
        }
        
        // Updates the role of a specific member in a room
        public async Task<bool> UpdateMemberRoleAsync(int roomId, int userId, string newRole)
        {
            var member = await _context.RoomMembers
                .AsTracking()
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.UserId == userId);
            if (member == null) return false;
            
            member.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }
        
        // ================================================================
        // UTILITY
        // ================================================================
        
        // Checks if a room exists by ID
        public async Task<bool> RoomExistsAsync(int roomId)
        {
            return await _context.ChatRooms.AnyAsync(r => r.Id == roomId);
        }
        
        // Manually saves all pending changes to the database
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}