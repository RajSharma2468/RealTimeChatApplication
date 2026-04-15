using Google.Apis.Auth;
using ConnectHub.Auth.DTOs;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        
        public GoogleAuthService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        
        public string GetGoogleLoginUrl()
        {
            var clientId = _configuration["Google:ClientId"];
            var redirectUri = "http://localhost:5046/api/auth/google-callback";
            
            return $"https://accounts.google.com/o/oauth2/v2/auth?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={redirectUri}&" +
                   $"response_type=code&" +
                   $"scope=email profile&" +
                   $"access_type=offline";
        }
        
        public async Task<UserResponseDto> HandleGoogleLogin(string code)
        {
            // Exchange code for tokens
            var tokenResponse = await ExchangeCodeForTokens(code);
            
            // Verify ID token
            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenResponse.IdToken);
            
            // Create or get user
            var email = payload.Email;
            var name = payload.Name;
            var googleId = payload.Subject;
            
            var existingUser = await _userService.GetUserByEmailAsync(email);
            
            if (existingUser == null)
            {
                var username = email.Split('@')[0] + new Random().Next(1000, 9999);
                var registerDto = new RegisterDto
                {
                    Username = username,
                    DisplayName = name ?? email.Split('@')[0],
                    Email = email,
                    Password = Guid.NewGuid().ToString()
                };
                return await _userService.RegisterGoogleUserAsync(registerDto, googleId);
            }
            
            return existingUser;
        }
        
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
            
            return System.Text.Json.JsonSerializer.Deserialize<GoogleTokenResponse>(json);
        }
    }
    
    public class GoogleTokenResponse
    {
        public string IdToken { get; set; }
        public string AccessToken { get; set; }
    }
}