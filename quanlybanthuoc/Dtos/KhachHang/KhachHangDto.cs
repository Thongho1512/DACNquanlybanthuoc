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

    /// <summary>
    /// DTO cho trang cá nhân khách hàng (Customer Profile)
    /// </summary>
    public class CustomerProfileDto
    {
        public int Id { get; set; }
        public string? TenKhachHang { get; set; }
        public string? Sdt { get; set; }
        public int? DiemTichLuy { get; set; }
        public DateOnly? NgayDangKy { get; set; }
        public int TongDonHang { get; set; }
        public decimal TongGiaTriMua { get; set; }
        public DateTime? LanMuaGanNhat { get; set; }
    }
}