namespace quanlybanthuoc.Dtos.LichSuDiem
{
    public class LichSuDiemDto
    {
        public int Id { get; set; }
        public int? IddonHang { get; set; }
        public int? IdkhachHang { get; set; }
        public int? DiemCong { get; set; }
        public int? DiemTru { get; set; }
        public DateOnly? NgayGiaoDich { get; set; }
    }

    /// <summary>
    /// DTO cho l?ch s? ?i?m tích l?y (Customer view)
    /// </summary>
    public class LichSuDiemCustomerDto
    {
        public int Id { get; set; }
        public int IdDonHang { get; set; }
        public int DiemCong { get; set; }
        public int DiemTru { get; set; }
        public int DiemConLai { get; set; }
        public DateOnly NgayGiaoDich { get; set; }
        public string? LoaiGiaoDich { get; set; } // "Mua", "S? d?ng"
        public decimal? GiaTriDonHang { get; set; }
    }
}
