namespace quanlybanthuoc.Dtos.DonHang
{
    public class DonHangDto
    {
        public int Id { get; set; }
        public int? IdnguoiDung { get; set; }
        public int? IdkhachHang { get; set; }
        public int? IdchiNhanh { get; set; }
        public int? IdphuongThucTt { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? TienGiamGia { get; set; }
        public decimal? ThanhTien { get; set; }
        public DateOnly? NgayTao { get; set; }

        // Navigation properties for display
        public string? TenNguoiDung { get; set; }
        public string? TenKhachHang { get; set; }
        public string? TenChiNhanh { get; set; }
        public string? TenPhuongThucTt { get; set; }

        // Chi tiết đơn hàng
        public List<ChiTietDonHangDto> ChiTietDonHangs { get; set; } = new();
    }

    public class ChiTietDonHangDto
    {
        public int Id { get; set; }
        public int? Idthuoc { get; set; }
        public string? TenThuoc { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
    }
}