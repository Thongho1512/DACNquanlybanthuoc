using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IKhachHangRepository : IBaseRepository<KhachHang>
    {
        Task<PagedResult<KhachHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(KhachHang entity);
        Task<KhachHang?> GetBySdtAsync(string sdt);
    }
}