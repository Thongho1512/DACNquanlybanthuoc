using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class KhoHangRepository : BaseRepository<KhoHang>, IKhoHangRepository
    {
        public KhoHangRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<KhoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            bool? tonKhoThap = null)
        {
            IQueryable<KhoHang> query = _dbSet
                .Include(kh => kh.IdchiNhanhNavigation)
                .Include(kh => kh.IdloHangNavigation)
                    .ThenInclude(lh => lh!.IdthuocNavigation)
                .AsQueryable();

            // Filter by chi nhánh
            if (idChiNhanh.HasValue)
            {
                query = query.Where(kh => kh.IdchiNhanh == idChiNhanh.Value);
            }

            // Filter tồn kho thấp
            if (tonKhoThap.HasValue && tonKhoThap.Value)
            {
                query = query.Where(kh =>
                    kh.SoLuongTon <= kh.TonKhoToiThieu);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(kh => kh.IdchiNhanh)
                .ThenBy(kh => kh.SoLuongTon)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<KhoHang>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public async Task<KhoHang?> GetByChiNhanhAndLoHangAsync(int idChiNhanh, int idLoHang)
        {
            return await _dbSet
                .Include(kh => kh.IdloHangNavigation)
                .FirstOrDefaultAsync(kh =>
                    kh.IdchiNhanh == idChiNhanh &&
                    kh.IdloHang == idLoHang);
        }

        public async Task<IEnumerable<KhoHang>> GetByChiNhanhIdAsync(int idChiNhanh)
        {
            return await _dbSet
                .Include(kh => kh.IdloHangNavigation)
                    .ThenInclude(lh => lh!.IdthuocNavigation)
                .Where(kh => kh.IdchiNhanh == idChiNhanh)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<KhoHang>> GetTonKhoThapAsync(int? idChiNhanh = null)
        {
            IQueryable<KhoHang> query = _dbSet
                .Include(kh => kh.IdchiNhanhNavigation)
                .Include(kh => kh.IdloHangNavigation)
                    .ThenInclude(lh => lh!.IdthuocNavigation)
                .Where(kh => kh.SoLuongTon <= kh.TonKhoToiThieu);

            if (idChiNhanh.HasValue)
            {
                query = query.Where(kh => kh.IdchiNhanh == idChiNhanh.Value);
            }

            return await query
                .OrderBy(kh => kh.SoLuongTon)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task TruTonKhoAsync(int idChiNhanh, int idLoHang, int soLuong)
        {
            var khoHang = await GetByChiNhanhAndLoHangAsync(idChiNhanh, idLoHang);

            if (khoHang == null)
            {
                throw new InvalidOperationException(
                    $"Không tìm thấy kho hàng cho chi nhánh {idChiNhanh} và lô hàng {idLoHang}");
            }

            if (khoHang.SoLuongTon < soLuong)
            {
                throw new InvalidOperationException(
                    $"Số lượng tồn kho không đủ. Còn lại: {khoHang.SoLuongTon}, yêu cầu: {soLuong}");
            }

            khoHang.SoLuongTon -= soLuong;
            khoHang.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);

            await UpdateAsync(khoHang);
        }

        public async Task CongTonKhoAsync(int idChiNhanh, int idLoHang, int soLuong)
        {
            var khoHang = await GetByChiNhanhAndLoHangAsync(idChiNhanh, idLoHang);

            if (khoHang == null)
            {
                throw new InvalidOperationException(
                    $"Không tìm thấy kho hàng cho chi nhánh {idChiNhanh} và lô hàng {idLoHang}");
            }

            khoHang.SoLuongTon += soLuong;
            khoHang.NgayCapNhat = DateOnly.FromDateTime(DateTime.Now);

            await UpdateAsync(khoHang);
        }

        public async Task DeleteAsync(KhoHang entity)
        {
            _dbSet.Remove(entity);
        }
    }
}