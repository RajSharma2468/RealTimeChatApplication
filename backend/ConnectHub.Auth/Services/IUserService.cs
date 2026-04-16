using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Services
{
    public interface IUserService
    {
        // Auth operations
        Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<UserResponseDto> RegisterGoogleUserAsync(RegisterDto registerDto, string googleId);  // ADD THIS
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(int userId);
        
        // Profile operations
        Task<UserResponseDto> GetProfileAsync(int userId);
        Task<UserResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto updateDto);
        
        // User lookup (for Room Service)
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        
        // Search
        Task<IEnumerable<SearchUserDto>> SearchUsersAsync(string keyword, int currentUserId);
    }
}