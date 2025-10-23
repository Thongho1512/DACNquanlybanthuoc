namespace quanlybanthuoc.Data.Entities;

public partial class NguoiDung
{
    public int Id { get; set; }

    public int? IdvaiTro { get; set; }

    public int? IdchiNhanh { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? HoTen { get; set; }

    public DateOnly? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<DonNhapHang> DonNhapHangs { get; set; } = new List<DonNhapHang>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ChiNhanh? IdchiNhanhNavigation { get; set; }

    public virtual VaiTro? IdvaiTroNavigation { get; set; }

}
