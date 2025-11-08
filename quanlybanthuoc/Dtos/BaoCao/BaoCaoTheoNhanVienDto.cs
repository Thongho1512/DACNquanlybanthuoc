namespace quanlybanthuoc.Dtos.BaoCao
{
    public class BaoCaoTheoNhanVienDto
    {
        public int IdNguoiDung { get; set; }
        public string? TenNhanVien { get; set; }
        public int? IdChiNhanh { get; set; }
        public string? TenChiNhanh { get; set; }
        public int SoDonHang { get; set; }
        public int TongSoLuongBan { get; set; }
        public decimal TongTienHang { get; set; }
        public decimal TongGiamGia { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal DoanhThuTrungBinh { get; set; }
        public decimal DonHangLonNhat { get; set; }
    }
}