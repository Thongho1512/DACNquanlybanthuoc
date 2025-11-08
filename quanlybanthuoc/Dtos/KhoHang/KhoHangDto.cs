namespace quanlybanthuoc.Dtos.KhoHang
{
    public class KhoHangDto
    {
        public int Id { get; set; }
        public int? IdchiNhanh { get; set; }
        public int? IdloHang { get; set; }
        public int? TonKhoToiThieu { get; set; }
        public int? SoLuongTon { get; set; }
        public DateOnly? NgayCapNhat { get; set; }

        // Navigation properties
        public string? TenChiNhanh { get; set; }
        public string? TenThuoc { get; set; }
        public string? SoLo { get; set; }
        public DateOnly? NgayHetHan { get; set; }
    }
}