namespace quanlybanthuoc.Dtos.DonNhapHang
{
    public class UpdateDonNhapHangDto
    {
        public int IdnhaCungCap { get; set; }
        public string SoDonNhap { get; set; } = string.Empty;
        public DateOnly NgayNhap { get; set; }
        public List<UpdateLoHangNhapDto> LoHangs { get; set; } = new();
    }

    public class UpdateLoHangNhapDto
    {
        public int? Id { get; set; } // Null nếu là lô mới
        public int Idthuoc { get; set; }
        public string SoLo { get; set; } = string.Empty;
        public DateOnly NgaySanXuat { get; set; }
        public DateOnly NgayHetHan { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
    }
}