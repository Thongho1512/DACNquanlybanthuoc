using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class KhachHang
{
    public int Id { get; set; }

    public string? TenKhachHang { get; set; }

    public string? Sdt { get; set; }

    public int? DiemTichLuy { get; set; }

    public DateOnly? NgayDangKy { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<LichSuDiem> LichSuDiems { get; set; } = new List<LichSuDiem>();
}
