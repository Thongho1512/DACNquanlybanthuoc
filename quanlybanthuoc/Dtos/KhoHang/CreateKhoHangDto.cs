namespace quanlybanthuoc.Dtos.KhoHang
{
    public class CreateKhoHangDto
    {
        public int IdchiNhanh { get; set; }
        public int IdloHang { get; set; }
        public int TonKhoToiThieu { get; set; } = 10;
        public int SoLuongTon { get; set; }
    }
}