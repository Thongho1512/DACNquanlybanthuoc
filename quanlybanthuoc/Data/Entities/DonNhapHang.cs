using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class DonNhapHang
{
    public int Id { get; set; }

    public int? IdchiNhanh { get; set; }

    public int? IdnhaCungCap { get; set; }

    public int? IdnguoiNhan { get; set; }

    public string? SoDonNhap { get; set; }

    public DateOnly? NgayNhap { get; set; }

    public decimal? TongTien { get; set; }

    public virtual ChiNhanh? IdchiNhanhNavigation { get; set; }

    public virtual NguoiDung? IdnguoiNhanNavigation { get; set; }

    public virtual NhaCungCap? IdnhaCungCapNavigation { get; set; }

    public virtual ICollection<LoHang> LoHangs { get; set; } = new List<LoHang>();
}
