using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IDanhMucRepository : IBaseRepository<DanhMuc>
    {
        Task<IEnumerable<DanhMuc>> GetAllAsync();
        Task<DanhMuc?> GetByTenDanhMucAsync(string tenDanhMuc);

        Task SoftDeleted(DanhMuc entity);
    }
}