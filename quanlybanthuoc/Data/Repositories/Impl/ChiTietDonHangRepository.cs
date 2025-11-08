using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class ChiTietDonHangRepository : BaseRepository<ChiTietDonHang>, IChiTietDonHangRepository
    {
        public ChiTietDonHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChiTietDonHang>> GetByDonHangIdAsync(int donHangId)
        {
            return await _dbSet
                .Include(ct => ct.IdthuocNavigation)
                .Where(ct => ct.IddonHang == donHangId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<ChiTietDonHang> chiTietDonHangs)
        {
            await _dbSet.AddRangeAsync(chiTietDonHangs);
        }
    }
}