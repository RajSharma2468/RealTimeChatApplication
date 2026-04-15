using ConnectHub.Auth.DTOs;

namespace ConnectHub.Auth.Services
{
    public interface IGoogleAuthService
    {
        Task<UserResponseDto> HandleGoogleLogin(string idToken);
        string GetGoogleLoginUrl();
    }
}