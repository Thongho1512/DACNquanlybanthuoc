namespace quanlybanthuoc.Dtos.DonGiaoHang
{
    public class UpdateDonGiaoHangDto
    {
        public int? IdnguoiGiaoHang { get; set; }
        public string? TrangThaiGiaoHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? SoDienThoaiNguoiNhan { get; set; }
        public string? TenNguoiNhan { get; set; }
        public decimal? PhiGiaoHang { get; set; }
        public string? GhiChu { get; set; }
    }

    public class AssignDeliveryPersonDto
    {
        public int IdnguoiGiaoHang { get; set; }
    }

    public class UpdateDeliveryStatusDto
    {
        public string TrangThaiGiaoHang { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
    }

    public class CancelDeliveryDto
    {
        public string LyDoHuy { get; set; } = string.Empty;
    }
}

