using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class ChiNhanh
{
    public int Id { get; set; }

    public string? TenChiNhanh { get; set; }

    public string? DiaChi { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<DonNhapHang> DonNhapHangs { get; set; } = new List<DonNhapHang>();

    public virtual ICollection<KhoHang> KhoHangs { get; set; } = new List<KhoHang>();

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

    public virtual ICollection<NguoiGiaoHang> NguoiGiaoHangs { get; set; } = new List<NguoiGiaoHang>();
}
