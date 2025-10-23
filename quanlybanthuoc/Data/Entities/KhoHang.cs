namespace quanlybanthuoc.Data.Entities;

public partial class KhoHang
{
    public int Id { get; set; }

    public int? IdchiNhanh { get; set; }

    public int? IdloHang { get; set; }

    public int? TonKhoToiThieu { get; set; }

    public int? SoLuongTon { get; set; }

    public DateOnly? NgayCapNhat { get; set; }

    public virtual ICollection<ChiTietLoHang> ChiTietLoHangs { get; set; } = new List<ChiTietLoHang>();

    public virtual ChiNhanh? IdchiNhanhNavigation { get; set; }

    public virtual LoHang? IdloHangNavigation { get; set; }
}
