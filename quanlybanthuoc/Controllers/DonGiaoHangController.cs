using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonGiaoHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/dongiaohangs")]
    [ApiController]
    [Authorize]
    public class DonGiaoHangsController : ControllerBase
    {
        private readonly ILogger<DonGiaoHangsController> _logger;
        private readonly IDonGiaoHangService _donGiaoHangService;

        public DonGiaoHangsController(
            ILogger<DonGiaoHangsController> logger,
            IDonGiaoHangService donGiaoHangService)
        {
            _logger = logger;
            _donGiaoHangService = donGiaoHangService;
        }

        /// <summary>
        /// Tạo đơn giao hàng mới - ADMIN, MANAGER
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> CreateDonGiaoHang([FromBody] CreateDonGiaoHangDto dto)
        {
            _logger.LogInformation("Creating new delivery order");
            var result = ApiResponse<DonGiaoHangDto>.SuccessResponse(
                await _donGiaoHangService.CreateAsync(dto));
            return CreatedAtAction(nameof(GetDonGiaoHangById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Xem chi tiết đơn giao hàng - TẤT CẢ NGƯỜI DÙNG
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetDonGiaoHangById(int id)
        {
            _logger.LogInformation($"Getting delivery order by id: {id}");
            var result = ApiResponse<DonGiaoHangDto>.SuccessResponse(
                await _donGiaoHangService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách đơn giao hàng - TẤT CẢ NGƯỜI DÙNG
        /// ADMIN/MANAGER: xem tất cả
        /// STAFF: chỉ xem đơn của chi nhánh mình
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetAllDonGiaoHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] int? idNguoiGiaoHang = null,
            [FromQuery] string? trangThaiGiaoHang = null,
            [FromQuery] DateOnly? tuNgay = null,
            [FromQuery] DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all delivery orders");

            // Nếu là STAFF, chỉ cho xem đơn của chi nhánh mình
            if (User.IsInRole("STAFF") || User.IsInRole("WAREHOUSE_STAFF"))
            {
                // TODO: Lấy IDChiNhanh từ user claim hoặc từ NguoiDung
                // Tạm thời để frontend truyền idChiNhanh
            }

            var result = ApiResponse<PagedResult<DonGiaoHangDto>>.SuccessResponse(
                await _donGiaoHangService.GetAllAsync(
                    pageNumber,
                    pageSize,
                    idChiNhanh,
                    idNguoiGiaoHang,
                    trangThaiGiaoHang,
                    tuNgay,
                    denNgay));

            return Ok(result);
        }

        /// <summary>
        /// Phân công người giao hàng - ADMIN, MANAGER
        /// </summary>
        [HttpPut("{id}/assign")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> AssignDeliveryPerson(int id, [FromBody] AssignDeliveryPersonDto dto)
        {
            _logger.LogInformation($"Assigning delivery person to delivery order {id}");
            var result = ApiResponse<DonGiaoHangDto>.SuccessResponse(
                await _donGiaoHangService.AssignDeliveryPersonAsync(id, dto));
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật trạng thái giao hàng - NGƯỜI GIAO HÀNG, ADMIN, MANAGER
        /// Người giao hàng có thể cập nhật trạng thái: DANG_CHUAN_BI, DANG_GIAO, DA_GIAO
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> UpdateDeliveryStatus(int id, [FromBody] UpdateDeliveryStatusDto dto)
        {
            _logger.LogInformation($"Updating delivery status for delivery order {id}");

            // Kiểm tra quyền: Người giao hàng chỉ có thể cập nhật đơn của mình
            var donGiaoHang = await _donGiaoHangService.GetByIdAsync(id);
            if (donGiaoHang == null)
            {
                return NotFound(ApiResponse<DonGiaoHangDto>.FailureResponse("Không tìm thấy đơn giao hàng"));
            }

            // Nếu không phải Admin/Manager, kiểm tra xem có phải người giao hàng của đơn này không
            if (!User.IsInRole("ADMIN") && !User.IsInRole("MANAGER"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<DonGiaoHangDto>.FailureResponse("Không tìm thấy thông tin người dùng"));
                }

                // TODO: Kiểm tra userId có phải là IdnguoiGiaoHang không
                // Tạm thời cho phép nếu có IdnguoiGiaoHang
                if (!donGiaoHang.IdnguoiGiaoHang.HasValue)
                {
                    return Forbid("Chỉ người giao hàng được phân công mới có thể cập nhật trạng thái");
                }
            }

            var result = ApiResponse<DonGiaoHangDto>.SuccessResponse(
                await _donGiaoHangService.UpdateStatusAsync(id, dto));
            return Ok(result);
        }

        /// <summary>
        /// Hủy đơn giao hàng - ADMIN, MANAGER
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> CancelDelivery(int id, [FromBody] CancelDeliveryDto dto)
        {
            _logger.LogInformation($"Cancelling delivery order {id}");
            var result = ApiResponse<DonGiaoHangDto>.SuccessResponse(
                await _donGiaoHangService.CancelAsync(id, dto));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách đơn giao hàng của người giao hàng - NGƯỜI GIAO HÀNG
        /// </summary>
        [HttpGet("my-deliveries")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            _logger.LogInformation("Getting delivery orders for current delivery person");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm thấy thông tin người dùng"));
            }

            // TODO: Lấy IdnguoiGiaoHang từ userId
            // Tạm thời dùng userId làm idNguoiGiaoHang (cần có bảng mapping)
            var result = ApiResponse<IEnumerable<DonGiaoHangDto>>.SuccessResponse(
                await _donGiaoHangService.GetByNguoiGiaoHangIdAsync(userId));

            return Ok(result);
        }
    }
}

