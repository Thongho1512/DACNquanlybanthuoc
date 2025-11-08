using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/donhangs")]
    [ApiController]
    [Authorize] // Tất cả endpoint yêu cầu đăng nhập
    public class DonHangsController : ControllerBase
    {
        private readonly ILogger<DonHangsController> _logger;
        private readonly IDonHangService _donHangService;

        public DonHangsController(ILogger<DonHangsController> logger, IDonHangService donHangService)
        {
            _logger = logger;
            _donHangService = donHangService;
        }

        /// <summary>
        /// UC11: Tạo hóa đơn - TẤT CẢ NHÂN VIÊN
        /// Theo tài liệu: Nhân viên bán hàng tạo hóa đơn thanh toán
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> CreateDonHang([FromBody] CreateDonHangDto dto)
        {
            _logger.LogInformation("Creating new order");

            // Lấy ID người dùng từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm thấy thông tin người dùng."));
            }

            int idNguoiDung = int.Parse(userIdClaim.Value);

            var result = ApiResponse<DonHangDto>.SuccessResponse(
                await _donHangService.CreateAsync(dto, idNguoiDung));

            return CreatedAtAction(nameof(GetDonHangById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Xem chi tiết đơn hàng - TẤT CẢ NGƯỜI DÙNG
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetDonHangById(int id)
        {
            _logger.LogInformation($"Getting order by id: {id}");
            var result = ApiResponse<DonHangDto>.SuccessResponse(await _donHangService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách đơn hàng - TẤT CẢ NGƯỜI DÙNG
        /// ADMIN/MANAGER: xem tất cả
        /// STAFF/WAREHOUSE_STAFF: chỉ xem đơn của chi nhánh mình
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetAllDonHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] int? idKhachHang = null,
            [FromQuery] DateOnly? tuNgay = null,
            [FromQuery] DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all orders");

            var result = ApiResponse<PagedResult<DonHangDto>>.SuccessResponse(
                await _donHangService.GetAllAsync(pageNumber, pageSize, idChiNhanh, idKhachHang, tuNgay, denNgay));

            return Ok(result);
        }

        /// <summary>
        /// UC12: Xóa hóa đơn - CHỈ ADMIN, MANAGER
        /// Theo tài liệu: Chỉ cấp quản lý mới được xóa hóa đơn
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteDonHang(int id)
        {
            _logger.LogInformation($"Deleting order with id: {id}");
            await _donHangService.DeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Đơn hàng đã được xóa thành công.");
            return Ok(result);
        }

        /// <summary>
        /// Xem lịch sử mua hàng của khách hàng - TẤT CẢ NGƯỜI DÙNG
        /// </summary>
        [HttpGet("khachhang/{khachHangId}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetDonHangsByKhachHang(int khachHangId)
        {
            _logger.LogInformation($"Getting orders by customer id: {khachHangId}");
            var result = ApiResponse<IEnumerable<DonHangDto>>.SuccessResponse(
                await _donHangService.GetByKhachHangIdAsync(khachHangId));
            return Ok(result);
        }
    }
}