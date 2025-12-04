using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class ChiTietDonHang
{
    public int Id { get; set; }

    public int? IddonHang { get; set; }

    public int? Idthuoc { get; set; }

    public int? SoLuong { get; set; }

    public decimal? DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual DonHang? IddonHangNavigation { get; set; }

    public virtual Thuoc? IdthuocNavigation { get; set; }
}
