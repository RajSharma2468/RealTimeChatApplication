using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ConnectHub.Auth.Models;

namespace ConnectHub.Auth.Helpers
{
    public interface IJwtHelper
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
    
    public class JwtHelper : IJwtHelper
    {
        // Configuration instance to read appsettings.json values
        private readonly IConfiguration _configuration;
        
        // Constructor - injects configuration
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // Generates a JWT token for authenticated users
        public string GenerateToken(User user)
        {
            // Create claims (user identity information)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            };
            
            // Get secret key from configuration and create security key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
            // Create signing credentials using HMAC SHA256 algorithm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            // Safely read expiry minutes from configuration with fallback default
            // Default is 1440 minutes (24 hours)
            var expiryMinutes = 1440;
            var expiryStr = _configuration["Jwt:ExpiryInMinutes"];
            if (!string.IsNullOrEmpty(expiryStr) && double.TryParse(expiryStr, out var minutes))
            {
                expiryMinutes = minutes;
            }
            
            // Create the JWT token with issuer, audience, claims, and expiry
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );
            
            // Write and return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        // Validates a JWT token and returns the user ID if valid
        public int? ValidateToken(string token)
        {
            // Return null if no token provided
            if (string.IsNullOrEmpty(token))
                return null;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            
            try
            {
                // Validate the token using the same parameters as generation
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                
                // Extract user ID from the validated token's claims
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                
                return userId;
            }
            catch
            {
                // Return null if token validation fails
                return null;
            }
        }
    }
}