namespace quanlybanthuoc.Data.Entities;

public partial class NhaCungCap
{
    public int Id { get; set; }

    public string? TenNhaCungCap { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonNhapHang> DonNhapHangs { get; set; } = new List<DonNhapHang>();
}
