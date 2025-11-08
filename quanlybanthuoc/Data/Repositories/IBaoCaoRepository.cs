using quanlybanthuoc.Dtos.BaoCao;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IBaoCaoRepository
    {
        Task<IEnumerable<BaoCaoDoanhThuTheoThangDto>> GetBaoCaoDoanhThuTheoThangAsync(
            int nam,
            int? thang = null,
            int? idChiNhanh = null);

        Task<IEnumerable<BaoCaoDoanhThuTheoNgayDto>> GetBaoCaoDoanhThuTheoNgayAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null);

        Task<IEnumerable<ThongKeThuocBanChayDto>> GetTopThuocBanChayAsync(
            int top = 10,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null,
            int? idChiNhanh = null);

        Task<ThongKeDashboardDto?> GetThongKeDashboardAsync(int? idChiNhanh = null);

        Task<IEnumerable<BaoCaoTheoNhanVienDto>> GetBaoCaoTheoNhanVienAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null);
    }
}