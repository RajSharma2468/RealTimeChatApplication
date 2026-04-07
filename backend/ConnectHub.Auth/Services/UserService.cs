using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Helpers;
using ConnectHub.Auth.Models;
using ConnectHub.Auth.Repositories;

namespace ConnectHub.Auth.Services
{
    public class UserService : IUserService
    {
        // Dependency Injection
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;
        
        public UserService(IUserRepository userRepository, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }
        
        public async Task<UserResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
                throw new Exception("Username already exists");
            
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                throw new Exception("Email already exists");
            
            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            await _userRepository.CreateAsync(user);
            
            return MapToResponseDto(user);
        }
        
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Find user by username or email
            var user = await _userRepository.GetByUsernameOrEmailAsync(loginDto.Username);
            
            if (user == null)
                throw new Exception("Invalid username or password");
            
            if (!user.IsActive)
                throw new Exception("Account is deactivated. Contact admin.");
            
            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");
            
            // Update last seen
            user.LastSeen = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            
            // Generate token
            var token = _jwtHelper.GenerateToken(user);
            
            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                DisplayName = user.DisplayName,
                UserId = user.Id,
                Message = "Login successful"
            };
        }
        
        public async Task<UserResponseDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                throw new Exception("User not found");
            
            return MapToResponseDto(user);
        }
        
        public async Task<UserResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                throw new Exception("User not found");
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(updateDto.DisplayName))
                user.DisplayName = updateDto.DisplayName;
            
            if (updateDto.Bio != null)
                user.Bio = updateDto.Bio;
            
            if (updateDto.AvatarUrl != null)
                user.AvatarUrl = updateDto.AvatarUrl;
            
            await _userRepository.UpdateAsync(user);
            
            return MapToResponseDto(user);
        }
        
        public async Task<IEnumerable<SearchUserDto>> SearchUsersAsync(string keyword, int currentUserId)
        {
            var users = await _userRepository.SearchUsersAsync(keyword, currentUserId);
            
            return users.Select(u => new SearchUserDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                IsOnline = false  // Will be updated by Presence Service later
            });
        }
        
        public async Task<bool> LogoutAsync(int userId)
        {
            // For JWT, logout is client-side
            // Just update last seen
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastSeen = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
            return true;
        }
        
        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastSeen = user.LastSeen
            };
        }
    }
}