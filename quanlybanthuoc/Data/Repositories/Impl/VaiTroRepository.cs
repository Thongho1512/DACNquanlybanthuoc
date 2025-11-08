using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

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
            vaiTro.TrangThai = false;
            _dbSet.Update(vaiTro);
            return Task.CompletedTask;
        }

        public async Task<PagedResult<VaiTro>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            IQueryable<VaiTro> query = _dbSet.AsQueryable();

            // Filter by status
            query = query.Where(vt => vt.TrangThai == active);

            // Search by name or description
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(vt =>
                    vt.TenVaiTro!.ToLower().Contains(searchTerm) ||
                    vt.MoTa!.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(vt => vt.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<VaiTro>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<IEnumerable<VaiTro>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(vt => vt.TrangThai == true)
                .OrderBy(vt => vt.TenVaiTro)
                .ToListAsync();
        }
    }
}