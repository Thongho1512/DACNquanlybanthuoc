using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Dtos.BaoCao;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class BaoCaoRepository : IBaoCaoRepository
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<BaoCaoRepository> _logger;

        public BaoCaoRepository(ShopDbContext context, ILogger<BaoCaoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BaoCaoDoanhThuTheoThangDto>> GetBaoCaoDoanhThuTheoThangAsync(
            int nam,
            int? thang = null,
            int? idChiNhanh = null)
        {
            _logger.LogInformation($"Executing sp_BaoCaoDoanhThuTheoThang - Nam: {nam}, Thang: {thang}, IDChiNhanh: {idChiNhanh}");

            var parameters = new[]
            {
                new SqlParameter("@Nam", nam),
                new SqlParameter("@Thang", (object?)thang ?? DBNull.Value),
                new SqlParameter("@IDChiNhanh", (object?)idChiNhanh ?? DBNull.Value)
            };

            var result = await _context.Database
                .SqlQueryRaw<BaoCaoDoanhThuTheoThangDto>(
                    "EXEC sp_BaoCaoDoanhThuTheoThang @Nam, @Thang, @IDChiNhanh",
                    parameters)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<BaoCaoDoanhThuTheoNgayDto>> GetBaoCaoDoanhThuTheoNgayAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null)
        {
            _logger.LogInformation($"Executing sp_BaoCaoDoanhThuTheoNgay - TuNgay: {tuNgay}, DenNgay: {denNgay}");

            var parameters = new[]
            {
                new SqlParameter("@TuNgay", tuNgay.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@DenNgay", denNgay.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@IDChiNhanh", (object?)idChiNhanh ?? DBNull.Value)
            };

            var result = await _context.Database
                .SqlQueryRaw<BaoCaoDoanhThuTheoNgayDto>(
                    "EXEC sp_BaoCaoDoanhThuTheoNgay @TuNgay, @DenNgay, @IDChiNhanh",
                    parameters)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<ThongKeThuocBanChayDto>> GetTopThuocBanChayAsync(
            int top = 10,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null,
            int? idChiNhanh = null)
        {
            _logger.LogInformation($"Executing sp_ThongKeTopThuocBanChay - Top: {top}");

            var parameters = new[]
            {
                new SqlParameter("@Top", top),
                new SqlParameter("@TuNgay", tuNgay.HasValue ? tuNgay.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value),
                new SqlParameter("@DenNgay", denNgay.HasValue ? denNgay.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value),
                new SqlParameter("@IDChiNhanh", (object?)idChiNhanh ?? DBNull.Value)
            };

            var result = await _context.Database
                .SqlQueryRaw<ThongKeThuocBanChayDto>(
                    "EXEC sp_ThongKeTopThuocBanChay @Top, @TuNgay, @DenNgay, @IDChiNhanh",
                    parameters)
                .ToListAsync();

            return result;
        }

        public async Task<ThongKeDashboardDto?> GetThongKeDashboardAsync(int? idChiNhanh = null)
        {
            _logger.LogInformation($"Executing sp_ThongKeDashboard - IDChiNhanh: {idChiNhanh}");

            var parameter = new SqlParameter("@IDChiNhanh", (object?)idChiNhanh ?? DBNull.Value);

            var result = await _context.Database
                .SqlQueryRaw<ThongKeDashboardDto>(
                    "EXEC sp_ThongKeDashboard @IDChiNhanh",
                    parameter)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<BaoCaoTheoNhanVienDto>> GetBaoCaoTheoNhanVienAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null)
        {
            _logger.LogInformation($"Executing sp_BaoCaoTheoNhanVien - TuNgay: {tuNgay}, DenNgay: {denNgay}");

            var parameters = new[]
            {
                new SqlParameter("@TuNgay", tuNgay.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@DenNgay", denNgay.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@IDChiNhanh", (object?)idChiNhanh ?? DBNull.Value)
            };

            var result = await _context.Database
                .SqlQueryRaw<BaoCaoTheoNhanVienDto>(
                    "EXEC sp_BaoCaoTheoNhanVien @TuNgay, @DenNgay, @IDChiNhanh",
                    parameters)
                .ToListAsync();

            return result;
        }
    }
}