using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class LichSuDiemRepository : BaseRepository<LichSuDiem>, ILichSuDiemRepository
    {
        public LichSuDiemRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LichSuDiem>> GetByKhachHangIdAsync(int khachHangId)
        {
            return await _dbSet
                .Include(ls => ls.IddonHangNavigation)
                .Where(ls => ls.IdkhachHang == khachHangId)
                .OrderByDescending(ls => ls.NgayGiaoDich)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LichSuDiem?> GetByDonHangIdAsync(int donHangId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ls => ls.IddonHang == donHangId);
        }

        public Task DeleteAsync(LichSuDiem entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
    }
}