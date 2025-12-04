using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class VwThongKeThuocBanChay
{
    public int ThuocId { get; set; }

    public string? TenThuoc { get; set; }

    public string? HoatChat { get; set; }

    public string? DonVi { get; set; }

    public decimal? GiaBan { get; set; }

    public string? TenDanhMuc { get; set; }

    public int? SoDonHang { get; set; }

    public int? TongSoLuongBan { get; set; }

    public decimal? TongDoanhThu { get; set; }

    public decimal? GiaTrungBinh { get; set; }
}
