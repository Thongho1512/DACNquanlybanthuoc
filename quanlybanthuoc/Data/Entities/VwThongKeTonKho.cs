using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class VwThongKeTonKho
{
    public int KhoHangId { get; set; }

    public int? IdchiNhanh { get; set; }

    public string? TenChiNhanh { get; set; }

    public int? Idthuoc { get; set; }

    public string? TenThuoc { get; set; }

    public string? DonVi { get; set; }

    public string? SoLo { get; set; }

    public DateOnly? NgayHetHan { get; set; }

    public int? SoLuongTon { get; set; }

    public int? TonKhoToiThieu { get; set; }

    public int TonKhoThap { get; set; }

    public int SapHetHan { get; set; }

    public int DaHetHan { get; set; }

    public decimal? GiaTriTonKho { get; set; }

    public DateOnly? NgayCapNhat { get; set; }
}
