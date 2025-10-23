namespace quanlybanthuoc.Dtos.NguoiDung
{
    public class UpdateNguoiDungDto
    {
        public int? IdvaiTro { get; set; }

        public int? IdchiNhanh { get; set; }

        public string? TenDangNhap { get; set; }

        public string? MatKhau { get; set; }

        public string? HoTen { get; set; }

        public DateOnly? NgayTao { get; set; }

        public bool? TrangThai { get; set; }
    }
}
