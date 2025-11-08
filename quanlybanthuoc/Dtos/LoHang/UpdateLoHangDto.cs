namespace quanlybanthuoc.Dtos.LoHang
{
    public class UpdateLoHangDto
    {
        public string? SoLo { get; set; }
        public DateOnly? NgaySanXuat { get; set; }
        public DateOnly? NgayHetHan { get; set; }
        public int? SoLuong { get; set; }
        public decimal? GiaNhap { get; set; }
    }
}