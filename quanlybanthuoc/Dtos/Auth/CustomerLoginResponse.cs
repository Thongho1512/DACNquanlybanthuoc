using quanlybanthuoc.Dtos.KhachHang;

namespace quanlybanthuoc.Dtos.Auth
{
    public class CustomerLoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public KhachHangDto KhachHangDto { get; set; } = new KhachHangDto();
    }
}

