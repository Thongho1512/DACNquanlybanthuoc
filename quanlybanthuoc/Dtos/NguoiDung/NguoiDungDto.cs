namespace quanlybanthuoc.Dtos.NguoiDung
{
    public class NguoiDungDto
    {
        public int Id { get; set; }

        public int? IdvaiTro { get; set; }

        public int? IdchiNhanh { get; set; }

        public string? TenDangNhap { get; set; }

        public string? HoTen { get; set; }

        public DateOnly? NgayTao { get; set; }

        public bool? TrangThai { get; set; }
    }
}
