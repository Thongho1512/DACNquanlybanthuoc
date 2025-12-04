namespace quanlybanthuoc.Dtos.ChiNhanh
{
    public class ChiNhanhDto
    {
        public int Id { get; set; }
        public string? TenChiNhanh { get; set; }
        public string? DiaChi { get; set; }
        public bool? TrangThai { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách chi nhánh trên trang customer (hiển thị thông tin cửa hàng)
    /// </summary>
    public class ChiNhanhCustomerDto
    {
        public int Id { get; set; }
        public string? TenChiNhanh { get; set; }
        public string? DiaChi { get; set; }
        public bool IsActive { get; set; }
        public int SoLuongSanPhamCoSan { get; set; }
    }
}