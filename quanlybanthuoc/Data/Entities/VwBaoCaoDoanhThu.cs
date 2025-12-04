using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class VwBaoCaoDoanhThu
{
    public int DonHangId { get; set; }

    public DateOnly? NgayTao { get; set; }

    public int? Nam { get; set; }

    public int? Thang { get; set; }

    public int? Ngay { get; set; }

    public int? IdchiNhanh { get; set; }

    public string? TenChiNhanh { get; set; }

    public int? IdnguoiDung { get; set; }

    public string? TenNhanVien { get; set; }

    public int? IdkhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public int? IdphuongThucTt { get; set; }

    public string? TenPhuongThuc { get; set; }

    public decimal? TongTien { get; set; }

    public decimal? TienGiamGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public int? SoLuongSanPham { get; set; }

    public int? TongSoLuong { get; set; }
}
