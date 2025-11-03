using quanlybanthuoc.Dtos.Auth;

namespace quanlybanthuoc.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(RefreshTokenRequest request);
    }
}
