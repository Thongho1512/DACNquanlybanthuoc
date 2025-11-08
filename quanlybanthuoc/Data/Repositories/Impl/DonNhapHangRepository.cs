using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class DonNhapHangRepository : BaseRepository<DonNhapHang>, IDonNhapHangRepository
    {
        public DonNhapHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DonNhapHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNhaCungCap = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            IQueryable<DonNhapHang> query = _dbSet
                .Include(dnh => dnh.IdchiNhanhNavigation)
                .Include(dnh => dnh.IdnhaCungCapNavigation)
                .Include(dnh => dnh.IdnguoiNhanNavigation)
                .AsQueryable();

            if (idChiNhanh.HasValue)
            {
                query = query.Where(dnh => dnh.IdchiNhanh == idChiNhanh.Value);
            }

            if (idNhaCungCap.HasValue)
            {
                query = query.Where(dnh => dnh.IdnhaCungCap == idNhaCungCap.Value);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(dnh => dnh.NgayNhap >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(dnh => dnh.NgayNhap <= denNgay.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(dnh => dnh.NgayNhap)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DonNhapHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<DonNhapHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(dnh => dnh.IdchiNhanhNavigation)
                .Include(dnh => dnh.IdnhaCungCapNavigation)
                .Include(dnh => dnh.IdnguoiNhanNavigation)
                .Include(dnh => dnh.LoHangs)
                    .ThenInclude(lh => lh.IdthuocNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(dnh => dnh.Id == id);
        }

        public async Task<DonNhapHang?> GetBySoDonNhapAsync(string soDonNhap)
        {
            return await _dbSet
                .FirstOrDefaultAsync(dnh => dnh.SoDonNhap == soDonNhap);
        }
    }
}