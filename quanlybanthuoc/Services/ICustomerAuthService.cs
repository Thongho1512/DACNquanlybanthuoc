using quanlybanthuoc.Dtos.Auth;

namespace quanlybanthuoc.Services
{
    public interface ICustomerAuthService
    {
        Task<CustomerLoginResponse> LoginAsync(CustomerLoginRequest request);
        Task<CustomerLoginResponse> RegisterAsync(CustomerRegisterRequest request);
        Task<string> SendOtpAsync(string sdt);
        Task<bool> VerifyOtpAsync(string sdt, string otp);
    }
}

