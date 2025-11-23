using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IDonNhapHangRepository : IBaseRepository<DonNhapHang>
    {
        Task<PagedResult<DonNhapHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNhaCungCap = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);

        Task<DonNhapHang?> GetByIdWithDetailsAsync(int id);
        Task<DonNhapHang?> GetBySoDonNhapAsync(string soDonNhap);
        Task DeleteAsync(DonNhapHang entity);
    }
}