namespace quanlybanthuoc.Data.Entities;

public partial class ChiTietLoHang
{
    public int Id { get; set; }

    public int? IdchiTietDh { get; set; }

    public int? IdkhoHang { get; set; }

    public int? SoLuong { get; set; }

    public virtual KhoHang? IdkhoHangNavigation { get; set; }
}
