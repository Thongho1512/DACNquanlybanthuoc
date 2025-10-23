namespace quanlybanthuoc.Data.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int? IdNguoiDung { get; set; }
        public string? Token { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayThuHoi { get; set; }

        public bool DaHetHan => DateTime.UtcNow >= NgayHetHan;
        public bool BiThuHoi => NgayThuHoi != null;
        public bool ConHieuLuc => !DaHetHan && !BiThuHoi;

        // Navigation property
        public NguoiDung? NguoiDung { get; set; }
    }
}
