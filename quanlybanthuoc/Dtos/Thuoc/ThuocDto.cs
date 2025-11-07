namespace quanlybanthuoc.Dtos.Thuoc
{
    public class ThuocDto
    {
        public int Id { get; set; }
        public int? IddanhMuc { get; set; }
        public string? TenThuoc { get; set; }
        public string? HoatChat { get; set; }
        public string? DonVi { get; set; }
        public decimal? GiaBan { get; set; }
        public string? MoTa { get; set; }
        public bool? TrangThai { get; set; }

        // Navigation properties for display
        public string? TenDanhMuc { get; set; }
    }
}