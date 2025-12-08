namespace quanlybanthuoc.Dtos.DonGiaoHang
{
    public class CreateDonGiaoHangDto
    {
        public int IddonHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? SoDienThoaiNguoiNhan { get; set; }
        public string? TenNguoiNhan { get; set; }
        public decimal? PhiGiaoHang { get; set; }
        public string? GhiChu { get; set; }
    }
}

