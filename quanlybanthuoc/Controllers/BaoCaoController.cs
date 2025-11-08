using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.BaoCao;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/baocao")]
    [ApiController]
    [Authorize] // Tất cả endpoint yêu cầu đăng nhập
    public class BaoCaoController : ControllerBase
    {
        private readonly ILogger<BaoCaoController> _logger;
        private readonly IBaoCaoService _baoCaoService;

        public BaoCaoController(
            ILogger<BaoCaoController> logger,
            IBaoCaoService baoCaoService)
        {
            _logger = logger;
            _baoCaoService = baoCaoService;
        }

        /// <summary>
        /// UC08: Báo cáo doanh thu theo tháng
        /// GET: /api/v1/baocao/doanh-thu/thang?nam=2025&thang=11&idChiNhanh=1
        /// </summary>
        [HttpGet("doanh-thu/thang")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetBaoCaoDoanhThuTheoThang(
            [FromQuery] int nam,
            [FromQuery] int? thang = null,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting monthly revenue report - Year: {nam}, Month: {thang}");

            var data = await _baoCaoService.GetBaoCaoDoanhThuTheoThangAsync(nam, thang, idChiNhanh);
            var result = ApiResponse<IEnumerable<BaoCaoDoanhThuTheoThangDto>>.SuccessResponse(data);

            return Ok(result);
        }

        /// <summary>
        /// UC08: Báo cáo doanh thu theo ngày
        /// GET: /api/v1/baocao/doanh-thu/ngay?tuNgay=2025-11-01&denNgay=2025-11-30&idChiNhanh=1
        /// </summary>
        [HttpGet("doanh-thu/ngay")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetBaoCaoDoanhThuTheoNgay(
            [FromQuery] DateOnly tuNgay,
            [FromQuery] DateOnly denNgay,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting daily revenue report - From: {tuNgay}, To: {denNgay}");

            var data = await _baoCaoService.GetBaoCaoDoanhThuTheoNgayAsync(tuNgay, denNgay, idChiNhanh);
            var result = ApiResponse<IEnumerable<BaoCaoDoanhThuTheoNgayDto>>.SuccessResponse(data);

            return Ok(result);
        }

        /// <summary>
        /// Thống kê top thuốc bán chạy
        /// GET: /api/v1/baocao/thuoc-ban-chay?top=10&tuNgay=2025-01-01&denNgay=2025-11-30
        /// </summary>
        [HttpGet("thuoc-ban-chay")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetTopThuocBanChay(
            [FromQuery] int top = 10,
            [FromQuery] DateOnly? tuNgay = null,
            [FromQuery] DateOnly? denNgay = null,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting top {top} best-selling medicines");

            var data = await _baoCaoService.GetTopThuocBanChayAsync(top, tuNgay, denNgay, idChiNhanh);
            var result = ApiResponse<IEnumerable<ThongKeThuocBanChayDto>>.SuccessResponse(data);

            return Ok(result);
        }

        /// <summary>
        /// Dashboard - Thống kê tổng quan
        /// GET: /api/v1/baocao/dashboard?idChiNhanh=1
        /// </summary>
        [HttpGet("dashboard")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetThongKeDashboard([FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting dashboard statistics");

            var data = await _baoCaoService.GetThongKeDashboardAsync(idChiNhanh);
            var result = ApiResponse<ThongKeDashboardDto>.SuccessResponse(data);

            return Ok(result);
        }

        /// <summary>
        /// Báo cáo hiệu suất theo nhân viên
        /// GET: /api/v1/baocao/nhan-vien?tuNgay=2025-11-01&denNgay=2025-11-30&idChiNhanh=1
        /// </summary>
        [HttpGet("nhan-vien")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> GetBaoCaoTheoNhanVien(
            [FromQuery] DateOnly tuNgay,
            [FromQuery] DateOnly denNgay,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting employee performance report - From: {tuNgay}, To: {denNgay}");

            var data = await _baoCaoService.GetBaoCaoTheoNhanVienAsync(tuNgay, denNgay, idChiNhanh);
            var result = ApiResponse<IEnumerable<BaoCaoTheoNhanVienDto>>.SuccessResponse(data);

            return Ok(result);
        }

        /// <summary>
        /// UC40: Xuất báo cáo doanh thu theo tháng (Excel)
        /// GET: /api/v1/baocao/xuat/doanh-thu-thang?nam=2025&thang=11&format=excel
        /// </summary>
        [HttpGet("xuat/doanh-thu-thang")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> ExportBaoCaoDoanhThuTheoThang(
            [FromQuery] int nam,
            [FromQuery] int? thang = null,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] string format = "excel")
        {
            _logger.LogInformation($"Exporting monthly revenue report - Format: {format}");

            var data = await _baoCaoService.GetBaoCaoDoanhThuTheoThangAsync(nam, thang, idChiNhanh);

            if (format.ToLower() == "excel")
            {
                // TODO: Implement Excel export using EPPlus or ClosedXML
                return Ok(new { message = "Excel export not yet implemented", data });
            }
            else if (format.ToLower() == "pdf")
            {
                // TODO: Implement PDF export using iTextSharp or QuestPDF
                return Ok(new { message = "PDF export not yet implemented", data });
            }

            return BadRequest(ApiResponse<string>.FailureResponse("Format không hợp lệ. Chỉ hỗ trợ 'excel' hoặc 'pdf'."));
        }
    }
}