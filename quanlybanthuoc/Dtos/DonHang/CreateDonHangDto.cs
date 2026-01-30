namespace quanlybanthuoc.Dtos.DonHang
{
    public class CreateDonHangDto
    {
        public int? IdkhachHang { get; set; }
        public int IdchiNhanh { get; set; }
        public int IdphuongThucTt { get; set; }
        public string? LoaiDonHang { get; set; } // "TAI_CHO" hoặc "GIAO_HANG"
        public List<ChiTietDonHangItemDto> ChiTietDonHangs { get; set; } = new();
    }

    public class ChiTietDonHangItemDto
    {
        public int Idthuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}