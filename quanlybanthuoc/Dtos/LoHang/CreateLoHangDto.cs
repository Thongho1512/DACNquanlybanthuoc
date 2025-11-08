namespace quanlybanthuoc.Dtos.LoHang
{
    public class CreateLoHangDto
    {
        public int Idthuoc { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateOnly NgaySanXuat { get; set; }
        public DateOnly NgayHetHan { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
    }
}