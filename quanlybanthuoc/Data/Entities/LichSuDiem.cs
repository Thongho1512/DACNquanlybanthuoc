namespace quanlybanthuoc.Data.Entities;

public partial class LichSuDiem
{
    public int Id { get; set; }

    public int? IdkhachHang { get; set; }

    public int? IddonHang { get; set; }

    public int? DiemCong { get; set; }

    public int? DiemTru { get; set; }

    public DateOnly? NgayGiaoDich { get; set; }

    public virtual DonHang? IddonHangNavigation { get; set; }

    public virtual KhachHang? IdkhachHangNavigation { get; set; }
}
