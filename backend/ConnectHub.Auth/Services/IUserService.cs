using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserResponseDto> GetProfileAsync(int userId);
        Task<UserResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto updateDto);
        Task<IEnumerable<SearchUserDto>> SearchUsersAsync(string keyword, int currentUserId);
        Task<bool> LogoutAsync(int userId);
    }
}