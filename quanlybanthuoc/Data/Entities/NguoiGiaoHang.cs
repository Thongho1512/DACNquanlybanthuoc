using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class NguoiGiaoHang
{
    public int Id { get; set; }

    public string? HoTen { get; set; }

    public string? Sdt { get; set; }

    public string? BienSoXe { get; set; }

    public int? IdchiNhanh { get; set; }

    public bool? TrangThai { get; set; }

    public DateOnly? NgayTao { get; set; }

    public virtual ICollection<DonGiaoHang> DonGiaoHangs { get; set; } = new List<DonGiaoHang>();

    public virtual ChiNhanh? IdchiNhanhNavigation { get; set; }
}
