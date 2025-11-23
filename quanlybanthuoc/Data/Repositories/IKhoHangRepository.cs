using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IKhoHangRepository : IBaseRepository<KhoHang>
    {
        Task<PagedResult<KhoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            bool? tonKhoThap = null);

        Task<KhoHang?> GetByChiNhanhAndLoHangAsync(int idChiNhanh, int idLoHang);
        Task<IEnumerable<KhoHang>> GetByChiNhanhIdAsync(int idChiNhanh);
        Task<IEnumerable<KhoHang>> GetTonKhoThapAsync(int? idChiNhanh = null);
        Task TruTonKhoAsync(int idChiNhanh, int idLoHang, int soLuong);
        Task CongTonKhoAsync(int idChiNhanh, int idLoHang, int soLuong);
        Task DeleteAsync(KhoHang entity);
    }
}