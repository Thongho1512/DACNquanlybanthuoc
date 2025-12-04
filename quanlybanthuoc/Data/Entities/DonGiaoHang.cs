using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class DonGiaoHang
{
    public int Id { get; set; }

    public int IddonHang { get; set; }

    public int? IdnguoiGiaoHang { get; set; }

    public string? TrangThaiGiaoHang { get; set; }

    public string? DiaChiGiaoHang { get; set; }

    public string? SoDienThoaiNguoiNhan { get; set; }

    public string? TenNguoiNhan { get; set; }

    public decimal? PhiGiaoHang { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayXacNhan { get; set; }

    public DateTime? NgayLayHang { get; set; }

    public DateTime? NgayBatDauGiao { get; set; }

    public DateTime? NgayGiaoThanhCong { get; set; }

    public string? GhiChu { get; set; }

    public string? LyDoHuy { get; set; }

    public virtual DonHang IddonHangNavigation { get; set; } = null!;

    public virtual NguoiGiaoHang? IdnguoiGiaoHangNavigation { get; set; }
}
