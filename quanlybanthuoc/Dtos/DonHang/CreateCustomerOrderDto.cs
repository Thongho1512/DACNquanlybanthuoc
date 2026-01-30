namespace quanlybanthuoc.Dtos.DonHang
{
    /// <summary>
    /// DTO cho khách hàng đặt hàng online (có thể không cần đăng nhập)
    /// </summary>
    public class CreateCustomerOrderDto
    {
        // Thông tin khách hàng (bắt buộc nếu không đăng nhập)
        public string? TenKhachHang { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }

        // Thông tin đơn hàng
        public int IdchiNhanh { get; set; }
        public int IdphuongThucTt { get; set; }
        public string? LoaiDonHang { get; set; } // "TAI_CHO" hoặc "GIAO_HANG"

        // Thông tin giao hàng (nếu LoaiDonHang = "GIAO_HANG")
        public string? DiaChiGiaoHang { get; set; }
        public string? SoDienThoaiNguoiNhan { get; set; }
        public string? TenNguoiNhan { get; set; }

        // Chi tiết đơn hàng
        public List<ChiTietDonHangItemDto> ChiTietDonHangs { get; set; } = new();

        // ID khách hàng (nếu đã đăng nhập, sẽ tự động lấy từ token)
        // Nếu có IdkhachHang thì sẽ tích điểm, nếu không thì không tích điểm
    }
}

