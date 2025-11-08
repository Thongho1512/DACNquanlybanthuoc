using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class ChiNhanhRepository : BaseRepository<ChiNhanh>, IChiNhanhRepository
    {
        public ChiNhanhRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<ChiNhanh>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null)
        {
            IQueryable<ChiNhanh> query = _dbSet.AsQueryable();

            query = query.Where(cn => cn.TrangThai == active);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(cn =>
                    cn.TenChiNhanh!.ToLower().Contains(searchTerm) ||
                    cn.DiaChi!.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(cn => cn.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ChiNhanh>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDeleteAsync(ChiNhanh entity)
        {
            entity.TrangThai = false;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}