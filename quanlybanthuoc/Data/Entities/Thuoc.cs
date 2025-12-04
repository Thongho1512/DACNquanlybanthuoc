using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class Thuoc
{
    public int Id { get; set; }

    public int? IddanhMuc { get; set; }

    public string? TenThuoc { get; set; }

    public string? HoatChat { get; set; }

    public string? DonVi { get; set; }

    public decimal? GiaBan { get; set; }

    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual DanhMuc? IddanhMucNavigation { get; set; }

    public virtual ICollection<LoHang> LoHangs { get; set; } = new List<LoHang>();
}
