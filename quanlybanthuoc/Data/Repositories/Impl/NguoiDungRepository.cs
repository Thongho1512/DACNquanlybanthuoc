using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class NguoiDungRepository : BaseRepository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(ShopDbContext context) : base(context)
        {

        }
        public async Task<NguoiDung?> GetByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(nguoiDung => nguoiDung.TenDangNhap == username);
        }

        public async Task<PagedResult<NguoiDung>> GetPagedListAsync(int pageNumber, int pageSize, bool active, string? searchTerm = null)
        {
            IQueryable<NguoiDung> query = _dbSet.AsQueryable();

            query = query.Where(nguoiDung => nguoiDung.TrangThai == active);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(nguoiDung =>
                    nguoiDung.HoTen!.ToLower().Contains(searchTerm) ||
                    nguoiDung.TenDangNhap!.ToLower().Contains(searchTerm));
            }

            var totalAmount  = await query.CountAsync();

            var items = query.AsNoTracking()
                .OrderBy(nguoiDung => nguoiDung.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<NguoiDung>
            {
                Items = items,
                TotalCount = totalAmount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDelete(NguoiDung entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
