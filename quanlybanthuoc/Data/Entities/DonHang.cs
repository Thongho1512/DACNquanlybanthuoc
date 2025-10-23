namespace quanlybanthuoc.Data.Entities;

public partial class DonHang
{
    public int Id { get; set; }

    public int? IdnguoiDung { get; set; }

    public int? IdkhachHang { get; set; }

    public int? IdchiNhanh { get; set; }

    public int? IdphuongThucTt { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? TienGiamGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public DateOnly? NgayTao { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ChiNhanh? IdchiNhanhNavigation { get; set; }

    public virtual KhachHang? IdkhachHangNavigation { get; set; }

    public virtual NguoiDung? IdnguoiDungNavigation { get; set; }

    public virtual PhuongThucThanhToan? IdphuongThucTtNavigation { get; set; }

    public virtual ICollection<LichSuDiem> LichSuDiems { get; set; } = new List<LichSuDiem>();
}
