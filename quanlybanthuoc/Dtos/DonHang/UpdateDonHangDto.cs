namespace quanlybanthuoc.Dtos.DonHang
{
    public class UpdateDonHangDto
    {
        public int? IdkhachHang { get; set; }
        public int IdphuongThucTt { get; set; }
        public List<UpdateChiTietDonHangItemDto> ChiTietDonHangs { get; set; } = new();
    }

    public class UpdateChiTietDonHangItemDto
    {
        public int? Id { get; set; } // Null nếu là chi tiết mới
        public int Idthuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}