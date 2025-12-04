using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class VaiTro
{
    public int Id { get; set; }

    public string? TenVaiTro { get; set; }

    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
