using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface INhaCungCapRepository : IBaseRepository<NhaCungCap>
    {
        Task<PagedResult<NhaCungCap>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(NhaCungCap entity);
    }
}