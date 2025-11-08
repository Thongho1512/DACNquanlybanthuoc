namespace quanlybanthuoc.Dtos.DonHang
{
    public class CreateDonHangDto
    {
        public int? IdkhachHang { get; set; }
        public int IdchiNhanh { get; set; }
        public int IdphuongThucTt { get; set; }
        public List<ChiTietDonHangItemDto> ChiTietDonHangs { get; set; } = new();
        public int? DiemSuDung { get; set; } // Điểm tích lũy khách hàng muốn sử dụng
    }

    public class ChiTietDonHangItemDto
    {
        public int Idthuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}