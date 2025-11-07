using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IThuocRepository : IBaseRepository<Thuoc>
    {
        Task<PagedResult<Thuoc>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null,
            int? idDanhMuc = null);

        Task SoftDeleteAsync(Thuoc entity);

        Task<IEnumerable<Thuoc>> GetThuocSapHetHanAsync(int days, int? idChiNhanh = null);

        Task<IEnumerable<Thuoc>> GetThuocTonKhoThapAsync(int? idChiNhanh = null);
    }
}