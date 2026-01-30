using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class DonGiaoHangRepository : BaseRepository<DonGiaoHang>, IDonGiaoHangRepository
    {
        public DonGiaoHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DonGiaoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNguoiGiaoHang = null,
            string? trangThaiGiaoHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            IQueryable<DonGiaoHang> query = _dbSet
                .Include(dg => dg.IddonHangNavigation)
                    .ThenInclude(dh => dh.IdchiNhanhNavigation)
                .Include(dg => dg.IdnguoiGiaoHangNavigation)
                .AsQueryable();

            // Filter by chi nhánh (thông qua đơn hàng)
            if (idChiNhanh.HasValue)
            {
                query = query.Where(dg => dg.IddonHangNavigation.IdchiNhanh == idChiNhanh.Value);
            }

            // Filter by người giao hàng
            if (idNguoiGiaoHang.HasValue)
            {
                query = query.Where(dg => dg.IdnguoiGiaoHang == idNguoiGiaoHang.Value);
            }

            // Filter by trạng thái
            if (!string.IsNullOrEmpty(trangThaiGiaoHang))
            {
                query = query.Where(dg => dg.TrangThaiGiaoHang == trangThaiGiaoHang);
            }

            // Filter by date range (theo ngày tạo)
            if (tuNgay.HasValue)
            {
                query = query.Where(dg => dg.NgayTao.HasValue && 
                    DateOnly.FromDateTime(dg.NgayTao.Value) >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(dg => dg.NgayTao.HasValue && 
                    DateOnly.FromDateTime(dg.NgayTao.Value) <= denNgay.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(dg => dg.NgayTao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DonGiaoHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<DonGiaoHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(dg => dg.IddonHangNavigation)
                    .ThenInclude(dh => dh.IdchiNhanhNavigation)
                .Include(dg => dg.IddonHangNavigation)
                    .ThenInclude(dh => dh.IdkhachHangNavigation)
                .Include(dg => dg.IdnguoiGiaoHangNavigation)
                    .ThenInclude(ng => ng.IdchiNhanhNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(dg => dg.Id == id);
        }

        public async Task<IEnumerable<DonGiaoHang>> GetByNguoiGiaoHangIdAsync(int idNguoiGiaoHang)
        {
            return await _dbSet
                .Include(dg => dg.IddonHangNavigation)
                .Where(dg => dg.IdnguoiGiaoHang == idNguoiGiaoHang)
                .OrderByDescending(dg => dg.NgayTao)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DonGiaoHang>> GetByDonHangIdAsync(int idDonHang)
        {
            return await _dbSet
                .Include(dg => dg.IdnguoiGiaoHangNavigation)
                .Where(dg => dg.IddonHang == idDonHang)
                .OrderByDescending(dg => dg.NgayTao)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

