using System;
using System.Collections.Generic;

namespace quanlybanthuoc.Data.Entities;

public partial class DanhMuc
{
    public int Id { get; set; }

    public string? TenDanhMuc { get; set; }

    public string? MoTa { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}
