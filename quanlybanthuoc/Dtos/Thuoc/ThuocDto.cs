namespace quanlybanthuoc.Dtos.Thuoc
{
    public class ThuocDto
    {
        public int Id { get; set; }
        public int? IddanhMuc { get; set; }
        public string? TenThuoc { get; set; }
        public string? HoatChat { get; set; }
        public string? DonVi { get; set; }
        public decimal? GiaBan { get; set; }
        public string? MoTa { get; set; }
        public bool? TrangThai { get; set; }

        // Navigation properties for display
        public string? TenDanhMuc { get; set; }
    }

    /// <summary>
    /// DTO cho hiển thị sản phẩm trên trang home/customer
    /// </summary>
    public class ThuocCustomerDto
    {
        public int Id { get; set; }
        public string? TenThuoc { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }
        public string? DonVi { get; set; }
        public string? HoatChat { get; set; }
        public string? TenDanhMuc { get; set; }
        public int? SoLuongConLai { get; set; }
        public bool? CoSan { get; set; }
        public DateTime? NgayCapNhatTonKho { get; set; }
    }

    /// <summary>
    /// DTO chi tiết sản phẩm (Product Detail Page)
    /// </summary>
    public class ThuocDetailDto
    {
        public int Id { get; set; }
        public string? TenThuoc { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }
        public string? DonVi { get; set; }
        public string? HoatChat { get; set; }
        public string? TenDanhMuc { get; set; }
        public int? IdDanhMuc { get; set; }
        public List<ThuocTonKhoByBranchDto> TonKhoTheoChiNhanh { get; set; } = new();
    }

    /// <summary>
    /// Tồn kho của sản phẩm tại từng chi nhánh
    /// </summary>
    public class ThuocTonKhoByBranchDto
    {
        public int IdChiNhanh { get; set; }
        public string? TenChiNhanh { get; set; }
        public int? SoLuongTon { get; set; }
        public bool? CoSan { get; set; }
    }

    /// <summary>
    /// DTO cho response tồn kho tại chi nhánh
    /// </summary>
    public class StockResponseDto
    {
        public int SoLuongTon { get; set; }
    }
}