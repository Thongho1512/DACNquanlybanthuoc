using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NguoiDung;

namespace quanlybanthuoc.Data.Repositories
{
    public interface INguoiDungRepository : IBaseRepository<NguoiDung>
    {
        Task<PagedResult<NguoiDung>> GetPagedListAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null);
        Task SoftDelete(NguoiDung entity);
        Task<NguoiDung?> GetByUsernameAsync(string username);
    }
}
