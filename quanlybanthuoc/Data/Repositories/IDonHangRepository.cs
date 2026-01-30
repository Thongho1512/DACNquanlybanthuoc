using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IDonHangRepository : IBaseRepository<DonHang>
    {
        Task<PagedResult<DonHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idKhachHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);

        Task<DonHang?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<DonHang>> GetByKhachHangIdAsync(int khachHangId);
        Task<IEnumerable<DonHang>> GetByChiNhanhIdAsync(int chiNhanhId, DateOnly? tuNgay = null, DateOnly? denNgay = null);
        Task DeleteAsync(DonHang entity);
        Task<DonHang?> GetByMomoOrderIdAsync(string momoOrderId);
    }
}