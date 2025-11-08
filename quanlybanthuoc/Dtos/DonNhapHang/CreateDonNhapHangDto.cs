namespace quanlybanthuoc.Dtos.DonNhapHang
{
    public class CreateDonNhapHangDto
    {
        public int IdchiNhanh { get; set; }
        public int IdnhaCungCap { get; set; }
        public string SoDonNhap { get; set; } = string.Empty;
        public DateOnly NgayNhap { get; set; }

        public List<LoHangNhapDto> LoHangs { get; set; } = new();
    }

    public class LoHangNhapDto
    {
        public int Idthuoc { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateOnly NgaySanXuat { get; set; }
        public DateOnly NgayHetHan { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
    }
}