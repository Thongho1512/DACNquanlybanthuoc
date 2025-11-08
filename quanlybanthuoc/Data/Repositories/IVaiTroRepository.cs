using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IVaiTroRepository : IBaseRepository<VaiTro>
    {
        Task<VaiTro?> GetByTenVaiTroAsync(string tenVaiTro);
        Task SoftDeleteAsync(VaiTro vaiTro);
        Task<PagedResult<VaiTro>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task<IEnumerable<VaiTro>> GetAllAsync();
    }
}