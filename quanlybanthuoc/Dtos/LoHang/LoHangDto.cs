namespace quanlybanthuoc.Dtos.LoHang
{
    public class LoHangDto
    {
        public int Id { get; set; }
        public int? IddonNhapHang { get; set; }
        public int? Idthuoc { get; set; }
        public string? SoLo { get; set; }
        public DateOnly? NgaySanXuat { get; set; }
        public DateOnly? NgayHetHan { get; set; }
        public int? SoLuong { get; set; }
        public decimal? GiaNhap { get; set; }

        // Navigation properties for display
        public string? TenThuoc { get; set; }
        public string? SoDonNhap { get; set; }
    }
}