namespace quanlybanthuoc.Data.Entities;

public partial class LoHang
{
    public int Id { get; set; }

    public int? IddonNhapHang { get; set; }

    public int? Idthuoc { get; set; }

    public string? SoLo { get; set; }

    public DateOnly? NgaySanXuat { get; set; }

    public DateOnly? NgayHetHan { get; set; }

    public int? SoLuong { get; set; }

    public decimal? GiaNhap { get; set; }

    public virtual DonNhapHang? IddonNhapHangNavigation { get; set; }

    public virtual Thuoc? IdthuocNavigation { get; set; }

    public virtual ICollection<KhoHang> KhoHangs { get; set; } = new List<KhoHang>();
}
