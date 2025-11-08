using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class KhachHangRepository : BaseRepository<KhachHang>, IKhachHangRepository
    {
        public KhachHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<KhachHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            IQueryable<KhachHang> query = _dbSet.AsQueryable();

            query = query.Where(kh => kh.TrangThai == active);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(kh =>
                    kh.TenKhachHang!.ToLower().Contains(searchTerm) ||
                    kh.Sdt!.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(kh => kh.NgayDangKy)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<KhachHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDeleteAsync(KhachHang entity)
        {
            entity.TrangThai = false;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<KhachHang?> GetBySdtAsync(string sdt)
        {
            return await _dbSet.FirstOrDefaultAsync(kh => kh.Sdt == sdt && kh.TrangThai == true);
        }
    }
}