namespace quanlybanthuoc.Data.Entities;

public partial class PhuongThucThanhToan
{
    public int Id { get; set; }

    public string? TenPhuongThuc { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
