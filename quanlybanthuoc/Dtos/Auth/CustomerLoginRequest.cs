namespace quanlybanthuoc.Dtos.Auth
{
    public class CustomerLoginRequest
    {
        public string Sdt { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty; // Mật khẩu
        public string? Otp { get; set; } // OTP nếu dùng OTP (tùy chọn, để tương thích)
    }

    public class CustomerRegisterRequest
    {
        public string TenKhachHang { get; set; } = string.Empty;
        public string Sdt { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty; // Mật khẩu
    }

    public class SendOtpRequest
    {
        public string Sdt { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        public string Sdt { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}

