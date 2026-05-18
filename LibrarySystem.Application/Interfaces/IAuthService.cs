using LibrarySystem.Application.DTOs.Auth;

namespace LibrarySystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}