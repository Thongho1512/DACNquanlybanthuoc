using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IVaiTroRepository : IBaseRepository<VaiTro>
    {
        Task<VaiTro?> GetByTenVaiTroAsync(string tenVaiTro);
        Task SoftDeleteAsync(VaiTro vaiTro);
    }
}
