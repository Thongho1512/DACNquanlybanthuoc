using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class PhuongThucThanhToanRepository : BaseRepository<PhuongThucThanhToan>, IPhuongThucThanhToanRepository
    {
        public PhuongThucThanhToanRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PhuongThucThanhToan>> GetAllAsync()
        {
            return await _dbSet
                .Where(pt => pt.TrangThai == true)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}