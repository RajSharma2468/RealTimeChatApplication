using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<IEnumerable<User>> SearchUsersAsync(string keyword, int currentUserId);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<int> SaveChangesAsync();
    }
}