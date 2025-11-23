using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class LoHangRepository : BaseRepository<LoHang>, ILoHangRepository
    {
        public LoHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<LoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idThuoc = null,
            int? idChiNhanh = null,
            bool? sapHetHan = null,
            int? daysToExpire = 30)
        {
            IQueryable<LoHang> query = _dbSet
                .Include(lh => lh.IdthuocNavigation)
                .Include(lh => lh.IddonNhapHangNavigation)
                .Include(lh => lh.KhoHangs)
                    .ThenInclude(kh => kh.IdchiNhanhNavigation)
                .AsQueryable();

            if (idThuoc.HasValue)
            {
                query = query.Where(lh => lh.Idthuoc == idThuoc.Value);
            }

            if (idChiNhanh.HasValue)
            {
                query = query.Where(lh =>
                    lh.KhoHangs.Any(kh => kh.IdchiNhanh == idChiNhanh.Value));
            }

            if (sapHetHan.HasValue && sapHetHan.Value)
            {
                var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
                var ngayGioiHan = ngayHienTai.AddDays(daysToExpire ?? 30);

                query = query.Where(lh =>
                    lh.NgayHetHan <= ngayGioiHan &&
                    lh.NgayHetHan > ngayHienTai);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(lh => lh.NgayHetHan)
                .ThenBy(lh => lh.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<LoHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        /// <summary>
        ///  FEFO: Lấy lô hàng theo thuốc và chi nhánh, sắp xếp theo HẠN SỬ DỤNG (sớm nhất trước)
        /// Chỉ lấy các lô còn tồn kho > 0 tại chi nhánh đó và chưa hết hạn
        /// </summary>
        public async Task<IEnumerable<LoHang>> GetByThuocIdAsync(int thuocId)
        {
            var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);

            return await _dbSet
                .Include(lh => lh.KhoHangs)
                .Include(lh => lh.IdthuocNavigation)
                .Where(lh =>
                    lh.Idthuoc == thuocId &&
                    lh.NgayHetHan > ngayHienTai) //  Chỉ lấy lô chưa hết hạn
                .OrderBy(lh => lh.NgayHetHan)  //  FEFO: Hết hạn sớm nhất đứng đầu
                .ThenBy(lh => lh.NgaySanXuat)  //  Nếu cùng HSD thì ưu tiên lô sản xuất sớm hơn
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<LoHang>> GetByDonNhapHangIdAsync(int donNhapHangId)
        {
            return await _dbSet
                .Include(lh => lh.IdthuocNavigation)
                .Where(lh => lh.IddonNhapHang == donNhapHangId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LoHang?> GetBySoLoAsync(string soLo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(lh => lh.SoLo == soLo);
        }

        public async Task DeleteAsync(LoHang entity)
        {
            _dbSet.Remove(entity);
        }
    }
}