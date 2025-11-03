using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class VaiTroRepository : BaseRepository<VaiTro>, IVaiTroRepository
    {
        public VaiTroRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<VaiTro?> GetByTenVaiTroAsync(string tenVaiTro)
        {
            return await _dbSet.FirstOrDefaultAsync(vt => vt.TenVaiTro == tenVaiTro);
        }

        public Task SoftDeleteAsync(VaiTro vaiTro)
        {
            var vaiTroEntity = _dbSet.FirstOrDefault(vt => vt.Id == vt.Id);
            vaiTroEntity!.TrangThai = false;
            return Task.CompletedTask;
        }
    }
}
