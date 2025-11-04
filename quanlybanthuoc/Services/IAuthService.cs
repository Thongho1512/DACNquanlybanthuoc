using quanlybanthuoc.Dtos.Auth;

namespace quanlybanthuoc.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string request);
        Task LogoutAsync(string request);
    }
}
