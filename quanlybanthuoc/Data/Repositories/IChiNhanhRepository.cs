using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IChiNhanhRepository : IBaseRepository<ChiNhanh>
    {
        Task<PagedResult<ChiNhanh>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(ChiNhanh entity);
    }
}