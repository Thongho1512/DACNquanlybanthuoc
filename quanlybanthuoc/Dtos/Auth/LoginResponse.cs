namespace quanlybanthuoc.Dtos.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string? VaiTro { get; set; }

    }
}
