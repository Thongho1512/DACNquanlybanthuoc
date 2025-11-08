using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using quanlybanthuoc.Data.Repositories;
using quanlybanthuoc.Dtos.BaoCao;

namespace quanlybanthuoc.Services.Impl
{
    public class BaoCaoService : IBaoCaoService
    {
        private readonly ILogger<BaoCaoService> _logger;
        private readonly IBaoCaoRepository _baoCaoRepository;

        public BaoCaoService(
            ILogger<BaoCaoService> logger,
            IBaoCaoRepository baoCaoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baoCaoRepository = baoCaoRepository ?? throw new ArgumentNullException(nameof(baoCaoRepository));
        }

        /// <summary>
        /// Báo cáo doanh thu theo tháng.
        /// </summary>
        public async Task<IEnumerable<BaoCaoDoanhThuTheoThangDto>> GetBaoCaoDoanhThuTheoThangAsync(
            int nam,
            int? thang = null,
            int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting monthly revenue report - Year: {Nam}, Month: {Thang}", nam, thang);
            return await _baoCaoRepository.GetBaoCaoDoanhThuTheoThangAsync(nam, thang, idChiNhanh);
        }

        /// <summary>
        /// Báo cáo doanh thu theo ngày trong khoảng thời gian.
        /// </summary>
        public async Task<IEnumerable<BaoCaoDoanhThuTheoNgayDto>> GetBaoCaoDoanhThuTheoNgayAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting daily revenue report - From: {TuNgay}, To: {DenNgay}", tuNgay, denNgay);

            if (tuNgay > denNgay)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

            return await _baoCaoRepository.GetBaoCaoDoanhThuTheoNgayAsync(tuNgay, denNgay, idChiNhanh);
        }

        /// <summary>
        /// Lấy danh sách thuốc bán chạy nhất.
        /// </summary>
        public async Task<IEnumerable<ThongKeThuocBanChayDto>> GetTopThuocBanChayAsync(
            int top = 10,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null,
            int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting top {Top} best-selling medicines", top);

            if (top <= 0 || top > 100)
                throw new ArgumentException("Top phải trong khoảng từ 1 đến 100.");

            return await _baoCaoRepository.GetTopThuocBanChayAsync(top, tuNgay, denNgay, idChiNhanh);
        }

        /// <summary>
        /// Thống kê dữ liệu tổng hợp cho Dashboard.
        /// </summary>
        public async Task<ThongKeDashboardDto?> GetThongKeDashboardAsync(int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting dashboard statistics");
            return await _baoCaoRepository.GetThongKeDashboardAsync(idChiNhanh);
        }

        /// <summary>
        /// Báo cáo hiệu suất nhân viên theo khoảng thời gian.
        /// </summary>
        public async Task<IEnumerable<BaoCaoTheoNhanVienDto>> GetBaoCaoTheoNhanVienAsync(
            DateOnly tuNgay,
            DateOnly denNgay,
            int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting employee performance report - From: {TuNgay}, To: {DenNgay}", tuNgay, denNgay);

            if (tuNgay > denNgay)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");

            return await _baoCaoRepository.GetBaoCaoTheoNhanVienAsync(tuNgay, denNgay, idChiNhanh);
        }
    }
}
