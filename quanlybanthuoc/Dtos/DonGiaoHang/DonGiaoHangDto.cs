namespace quanlybanthuoc.Dtos.DonGiaoHang
{
    public class DonGiaoHangDto
    {
        public int Id { get; set; }
        public int IddonHang { get; set; }
        public int? IdnguoiGiaoHang { get; set; }
        public string? TrangThaiGiaoHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? SoDienThoaiNguoiNhan { get; set; }
        public string? TenNguoiNhan { get; set; }
        public decimal? PhiGiaoHang { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public DateTime? NgayLayHang { get; set; }
        public DateTime? NgayBatDauGiao { get; set; }
        public DateTime? NgayGiaoThanhCong { get; set; }
        public string? GhiChu { get; set; }
        public string? LyDoHuy { get; set; }
        public string? TenNguoiGiaoHang { get; set; }
        public string? SdtNguoiGiaoHang { get; set; }
    }

    /// <summary>
    /// DTO cho tracking ??n hàng (Customer view)
    /// </summary>
    public class ShipmentTrackingDto
    {
        public int Id { get; set; }
        public int IdDonHang { get; set; }
        public string? TrangThaiGiaoHang { get; set; } // PENDING, CONFIRMED, PICKING, SHIPPING, DELIVERED, CANCELLED
        public string? TenNguoiGiaoHang { get; set; }
        public string? SdtNguoiGiaoHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? TenNguoiNhan { get; set; }
        public decimal? PhiGiaoHang { get; set; }
        public List<ShipmentHistoryDto> LichSuCapNhat { get; set; } = new();
    }

    /// <summary>
    /// L?ch s? c?p nh?t tr?ng thái giao hàng
    /// </summary>
    public class ShipmentHistoryDto
    {
        public string? TrangThai { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string? MoTa { get; set; }
    }
}
