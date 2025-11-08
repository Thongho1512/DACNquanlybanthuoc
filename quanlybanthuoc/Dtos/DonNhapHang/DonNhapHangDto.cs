namespace quanlybanthuoc.Dtos.DonNhapHang
{
    public class DonNhapHangDto
    {
        public int Id { get; set; }
        public int? IdchiNhanh { get; set; }
        public int? IdnhaCungCap { get; set; }
        public int? IdnguoiNhan { get; set; }
        public string? SoDonNhap { get; set; }
        public DateOnly? NgayNhap { get; set; }
        public decimal? TongTien { get; set; }

        // Navigation properties
        public string? TenChiNhanh { get; set; }
        public string? TenNhaCungCap { get; set; }
        public string? TenNguoiNhan { get; set; }

        // Chi tiết lô hàng
        public List<LoHangItemDto> LoHangs { get; set; } = new();
    }

    public class LoHangItemDto
    {
        public int Id { get; set; }
        public int Idthuoc { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateOnly NgaySanXuat { get; set; }
        public DateOnly NgayHetHan { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal ThanhTien { get; set; }
    }
}