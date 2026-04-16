using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Google.Apis.Auth;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Services;
using ConnectHub.Auth.Helpers;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Dependency Injection
        private readonly IUserService _userService;
        private readonly IJwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        
        public AuthController(IUserService userService, IJwtHelper jwtHelper, IConfiguration configuration)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }
        
        [HttpOptions]
        [AllowAnonymous]
        public IActionResult Options()
        {
            Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
            Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
            Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            return Ok();
        }
        
        // ================================================================
        // REGISTER
        // ================================================================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _userService.RegisterAsync(registerDto);
                return Ok(new { success = true, data = result, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // LOGIN
        // ================================================================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(loginDto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GOOGLE LOGIN - Redirect to Google OAuth page
        // ================================================================
        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var clientId = _configuration["Google:ClientId"];
            var redirectUri = "http://localhost:5046/api/auth/google-callback";
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                      $"client_id={clientId}&" +
                      $"redirect_uri={redirectUri}&" +
                      $"response_type=code&" +
                      $"scope=email profile&" +
                      $"access_type=offline";
            
            return Redirect(url);
        }
        
        // ================================================================
        // GOOGLE CALLBACK - Handle Google OAuth callback
        // ================================================================
        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest(new { success = false, message = "No authorization code provided" });
                }
                
                // Exchange code for tokens
                var tokenResponse = await ExchangeCodeForTokens(code);
                
                if (string.IsNullOrEmpty(tokenResponse?.id_token))
                {
                    return BadRequest(new { success = false, message = "Failed to get ID token from Google" });
                }
                
                // Verify ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.id_token);
                
                var email = payload.Email;
                var name = payload.Name;
                var googleId = payload.Subject;
                
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { success = false, message = "Email not provided by Google" });
                }
                
                // Check if user already exists
                var existingUser = await _userService.GetUserByEmailAsync(email);
                UserResponseDto user;
                
                if (existingUser == null)
                {
                    // Create new user from Google data
                    var username = email.Split('@')[0] + new Random().Next(1000, 9999).ToString();
                    var registerDto = new RegisterDto
                    {
                        Username = username,
                        DisplayName = name ?? email.Split('@')[0],
                        Email = email,
                        Password = Guid.NewGuid().ToString()
                    };
                    
                    user = await _userService.RegisterGoogleUserAsync(registerDto, googleId);
                }
                else
                {
                    user = existingUser;
                }
                
                // Generate JWT token
                var token = _jwtHelper.GenerateToken(new User 
                { 
                    Id = user.Id, 
                    Username = user.Username,
                    DisplayName = user.DisplayName,
                    Email = user.Email 
                });
                
                // Redirect back to frontend with token
                var frontendUrl = $"http://localhost:3000/auth/google-callback?token={token}&userId={user.Id}&displayName={Uri.EscapeDataString(user.DisplayName)}&username={user.Username}";
                
                return Redirect(frontendUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // Exchange authorization code for tokens
        // ================================================================
        private async Task<GoogleTokenResponse> ExchangeCodeForTokens(string code)
        {
            var clientId = _configuration["Google:ClientId"];
            var clientSecret = _configuration["Google:ClientSecret"];
            var redirectUri = "http://localhost:5046/api/auth/google-callback";
            
            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });
            
            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
            var json = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<GoogleTokenResponse>(json);
        }
        
        // ================================================================
        // GET USER BY EMAIL
        // ================================================================
        [HttpGet("user/by-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });
                
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // LOGOUT
        // ================================================================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return BadRequest(new { success = false, message = "User not authenticated" });
            
            var userId = int.Parse(userIdClaim);
            await _userService.LogoutAsync(userId);
            return Ok(new { success = true, message = "Logout successful" });
        }
        
        // ================================================================
        // GET USER BY ID
        // ================================================================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });
                
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // GET CURRENT USER PROFILE
        // ================================================================
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest(new { success = false, message = "User not authenticated" });
                
                var userId = int.Parse(userIdClaim);
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found" });
                
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // UPDATE PROFILE
        // ================================================================
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return BadRequest(new { success = false, message = "User not authenticated" });
                
                var userId = int.Parse(userIdClaim);
                var result = await _userService.UpdateProfileAsync(userId, updateDto);
                return Ok(new { success = true, data = result, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        
        // ================================================================
        // SEARCH USERS
        // ================================================================
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q) || q.Length < 2)
                return Ok(new { success = true, data = new List<SearchUserDto>() });
            
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserId = string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
                
                var users = await _userService.SearchUsersAsync(q, currentUserId);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
    
    // ================================================================
    // Google Token Response Class
    // ================================================================
    public class GoogleTokenResponse
    {
        public string id_token { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
    }
}