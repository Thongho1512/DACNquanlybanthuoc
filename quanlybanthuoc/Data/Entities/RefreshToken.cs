using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int? IdNguoiDung { get; set; }

    public string? Token { get; set; }

    public DateTime? NgayHetHan { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgayThuHoi { get; set; }

    public bool? DaHetHan { get; set; }

    public bool? BiThuHoi { get; set; }

    public bool? ConHieuLuc { get; set; }

    // Navigation property to user
    public virtual NguoiDung? NguoiDung { get; set; }
}
