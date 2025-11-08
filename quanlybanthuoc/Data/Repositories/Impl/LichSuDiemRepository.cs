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
    }
}