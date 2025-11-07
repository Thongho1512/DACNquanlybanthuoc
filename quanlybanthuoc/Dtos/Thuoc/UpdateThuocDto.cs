namespace quanlybanthuoc.Dtos.Thuoc
{
    public class UpdateThuocDto
    {
        public int? IddanhMuc { get; set; }
        public string? TenThuoc { get; set; }
        public string? HoatChat { get; set; }
        public string? DonVi { get; set; }
        public decimal? GiaBan { get; set; }
        public string? MoTa { get; set; }
        public bool? TrangThai { get; set; }
    }
}