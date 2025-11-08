namespace quanlybanthuoc.Dtos.KhachHang
{
    public class KhachHangDto
    {
        public int Id { get; set; }
        public string? TenKhachHang { get; set; }
        public string? Sdt { get; set; }
        public int? DiemTichLuy { get; set; }
        public DateOnly? NgayDangKy { get; set; }
        public bool? TrangThai { get; set; }
    }
}