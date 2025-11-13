using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class ThuocRepository : BaseRepository<Thuoc>, IThuocRepository
    {
        public ThuocRepository(ShopDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Thuoc>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null,
            int? idDanhMuc = null)
        {
            IQueryable<Thuoc> query = _dbSet.AsQueryable();

            // Filter by status
            query = query.Where(thuoc => thuoc.TrangThai == active);

            // Filter by category
            if (idDanhMuc.HasValue)
            {
                query = query.Where(thuoc => thuoc.IddanhMuc == idDanhMuc.Value);
            }

            // Search by name or active ingredient
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(thuoc =>
                    thuoc.TenThuoc!.ToLower().Contains(searchTerm) ||
                    thuoc.HoatChat!.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .Include(thuoc => thuoc.IddanhMucNavigation)
                .OrderBy(thuoc => thuoc.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Thuoc>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }

        public Task SoftDeleteAsync(Thuoc entity)
        {
            entity.TrangThai = false;
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Thuoc>> GetThuocSapHetHanAsync(int days, int? idChiNhanh = null)
        {
            var ngayHienTai = DateOnly.FromDateTime(DateTime.Now);
            var ngayGioiHan = ngayHienTai.AddDays(days);

            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.LoHangs)
                    .ThenInclude(lh => lh.KhoHangs)
                .Where(t => t.TrangThai == true &&
                       t.LoHangs.Any(lh =>
                           lh.NgayHetHan <= ngayGioiHan &&
                           lh.NgayHetHan > ngayHienTai));

            if (idChiNhanh.HasValue)
            {
                query = query.Where(t =>
                    t.LoHangs.Any(lh =>
                        lh.KhoHangs.Any(kh => kh.IdchiNhanh == idChiNhanh.Value)));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Thuoc>> GetThuocTonKhoThapAsync(int? idChiNhanh = null)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(t => t.LoHangs)
                    .ThenInclude(lh => lh.KhoHangs)
                .Where(t => t.TrangThai == true &&
                       t.LoHangs.Any(lh =>
                           lh.KhoHangs.Any(kh =>
                               kh.SoLuongTon <= kh.TonKhoToiThieu)));

            if (idChiNhanh.HasValue)
            {
                query = query.Where(t =>
                    t.LoHangs.Any(lh =>
                        lh.KhoHangs.Any(kh => kh.IdchiNhanh == idChiNhanh.Value)));
            }

            return await query.ToListAsync();
        }

        public async Task<PagedResult<Thuoc>> GetByChiNhanhIdAsync(
    int idChiNhanh,
    int pageNumber,
    int pageSize,
    bool active,
    string? searchTerm = null,
    int? idDanhMuc = null)
        {
            // Query thuốc có tồn tại trong kho hàng của chi nhánh
            IQueryable<Thuoc> query = _dbSet
                .Include(t => t.IddanhMucNavigation)
                .Include(t => t.LoHangs)
                    .ThenInclude(lh => lh.KhoHangs)
                .Where(t => t.LoHangs.Any(lh =>
                    lh.KhoHangs.Any(kh =>
                        kh.IdchiNhanh == idChiNhanh &&
                        kh.SoLuongTon > 0)));

            // Filter by status
            query = query.Where(thuoc => thuoc.TrangThai == active);

            // Filter by category
            if (idDanhMuc.HasValue)
            {
                query = query.Where(thuoc => thuoc.IddanhMuc == idDanhMuc.Value);
            }

            // Search by name or active ingredient
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(thuoc =>
                    thuoc.TenThuoc!.ToLower().Contains(searchTerm) ||
                    thuoc.HoatChat!.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .OrderBy(thuoc => thuoc.TenThuoc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Thuoc>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            };
        }
    }
}