namespace quanlybanthuoc.Dtos.BaoCao
{
    public class BaoCaoDoanhThuTheoNgayDto
    {
        public DateOnly Ngay { get; set; }
        public int? IdChiNhanh { get; set; }
        public string? TenChiNhanh { get; set; }
        public int SoDonHang { get; set; }
        public int TongSoLuongSanPham { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TongTienGiamGia { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal DoanhThuTrungBinh { get; set; }
    }
}