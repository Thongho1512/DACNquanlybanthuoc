using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class DanhMucRepository : BaseRepository<DanhMuc>, IDanhMucRepository
    {
        public DanhMucRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DanhMuc>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().OrderBy(dm => dm.TenDanhMuc).ToListAsync();
        }

        public async Task<DanhMuc?> GetByTenDanhMucAsync(string tenDanhMuc)
        {
            return await _dbSet.FirstOrDefaultAsync(dm => dm.TenDanhMuc == tenDanhMuc);
        }

        public Task SoftDeleted(DanhMuc entity)
        {
            entity.TrangThai = false;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}