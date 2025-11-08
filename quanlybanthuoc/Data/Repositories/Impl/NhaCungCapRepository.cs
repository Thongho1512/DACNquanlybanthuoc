using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class NhaCungCapRepository : BaseRepository<NhaCungCap>, INhaCungCapRepository
    {
        public NhaCungCapRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<NhaCungCap>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            IQueryable<NhaCungCap> query = _dbSet.AsQueryable();

            query = query.Where(ncc => ncc.TrangThai == active);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(ncc =>
                    ncc.TenNhaCungCap!.ToLower().Contains(searchTerm) ||
                    ncc.Sdt!.Contains(searchTerm) ||
                    ncc.Email!.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(ncc => ncc.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<NhaCungCap>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDeleteAsync(NhaCungCap entity)
        {
            entity.TrangThai = false;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}