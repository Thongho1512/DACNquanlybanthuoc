using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class DonHangRepository : BaseRepository<DonHang>, IDonHangRepository
    {
        public DonHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<DonHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idKhachHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null)
        {
            IQueryable<DonHang> query = _dbSet
                .Include(dh => dh.IdnguoiDungNavigation)
                .Include(dh => dh.IdkhachHangNavigation)
                .Include(dh => dh.IdchiNhanhNavigation)
                .Include(dh => dh.IdphuongThucTtNavigation)
                .AsQueryable();

            // Filter by chi nhánh
            if (idChiNhanh.HasValue)
            {
                query = query.Where(dh => dh.IdchiNhanh == idChiNhanh.Value);
            }

            // Filter by khách hàng
            if (idKhachHang.HasValue)
            {
                query = query.Where(dh => dh.IdkhachHang == idKhachHang.Value);
            }

            // Filter by date range
            if (tuNgay.HasValue)
            {
                query = query.Where(dh => dh.NgayTao >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(dh => dh.NgayTao <= denNgay.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderByDescending(dh => dh.NgayTao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DonHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<DonHang?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(dh => dh.IdnguoiDungNavigation)
                .Include(dh => dh.IdkhachHangNavigation)
                .Include(dh => dh.IdchiNhanhNavigation)
                .Include(dh => dh.IdphuongThucTtNavigation)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdthuocNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(dh => dh.Id == id);
        }

        public async Task<IEnumerable<DonHang>> GetByKhachHangIdAsync(int khachHangId)
        {
            return await _dbSet
                .Include(dh => dh.ChiTietDonHangs)
                .Where(dh => dh.IdkhachHang == khachHangId)
                .OrderByDescending(dh => dh.NgayTao)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<DonHang>> GetByChiNhanhIdAsync(int chiNhanhId, DateOnly? tuNgay = null, DateOnly? denNgay = null)
        {
            IQueryable<DonHang> query = _dbSet.Where(dh => dh.IdchiNhanh == chiNhanhId);

            if (tuNgay.HasValue)
            {
                query = query.Where(dh => dh.NgayTao >= tuNgay.Value);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(dh => dh.NgayTao <= denNgay.Value);
            }

            return await query
                .Include(dh => dh.ChiTietDonHangs)
                .OrderByDescending(dh => dh.NgayTao)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// ✅ FIX: Xóa đơn hàng bằng raw SQL để tránh tracking conflict
        /// </summary>
        public async Task DeleteAsync(DonHang entity)
        {
            // CÁCH 1: Raw SQL (RECOMMENDED - Tránh hoàn toàn tracking conflict)
            await _context.Database.ExecuteSqlRawAsync(
                $"DELETE FROM DonHang WHERE ID = {entity.Id}");

        }

        public async Task<DonHang?> GetByMomoOrderIdAsync(string momoOrderId)
        {
            return await _dbSet
                .Include(dh => dh.DonGiaoHangs)
                .FirstOrDefaultAsync(dh => dh.MomoOrderId == momoOrderId);
        }
    }
}