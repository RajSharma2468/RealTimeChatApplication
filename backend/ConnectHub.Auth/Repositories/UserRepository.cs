using Microsoft.EntityFrameworkCore;
using ConnectHub.Auth.Models;
using ConnectHub.Auth.Data;

namespace ConnectHub.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }
        
        /// Search users by keyword - excludes current user
        public async Task<IEnumerable<User>> SearchUsersAsync(string keyword, int currentUserId)
        {
            // Return empty if keyword is too short
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 2)
                return new List<User>();
            
            return await _context.Users
                .Where(u => u.Id != currentUserId && 
                       (u.Username.Contains(keyword) || 
                        u.DisplayName.Contains(keyword) ||
                        u.Email.Contains(keyword)))
                .Take(20)
                .ToListAsync();
        }
        
        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}