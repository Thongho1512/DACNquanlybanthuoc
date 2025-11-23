using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonNhapHang;
using quanlybanthuoc.Services;
using System.Security.Claims;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/donnhaphangs")]
    [ApiController]
    [Authorize(Policy = "WarehouseStaff")] // Admin, Manager, Warehouse Staff
    public class DonNhapHangsController : ControllerBase
    {
        private readonly ILogger<DonNhapHangsController> _logger;
        private readonly IDonNhapHangService _donNhapHangService;

        public DonNhapHangsController(
            ILogger<DonNhapHangsController> logger,
            IDonNhapHangService donNhapHangService)
        {
            _logger = logger;
            _donNhapHangService = donNhapHangService;
        }

        /// <summary>
        /// Tạo đơn nhập hàng - WAREHOUSE_STAFF
        /// Theo tài liệu: Nhân viên kho nhập hàng, xuất hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDonNhapHang([FromBody] CreateDonNhapHangDto dto)
        {
            _logger.LogInformation("Creating new import order");

            // Lấy ID người dùng từ token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(ApiResponse<string>.FailureResponse("Không tìm thấy thông tin người dùng."));
            }

            int idNguoiDung = int.Parse(userIdClaim.Value);

            var result = ApiResponse<DonNhapHangDto>.SuccessResponse(
                await _donNhapHangService.CreateAsync(dto, idNguoiDung));

            return CreatedAtAction(nameof(GetDonNhapHangById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Xem chi tiết đơn nhập hàng - WAREHOUSE_STAFF
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonNhapHangById(int id)
        {
            _logger.LogInformation($"Getting import order by id: {id}");
            var result = ApiResponse<DonNhapHangDto>.SuccessResponse(
                await _donNhapHangService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách đơn nhập hàng - WAREHOUSE_STAFF
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllDonNhapHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] int? idNhaCungCap = null,
            [FromQuery] DateOnly? tuNgay = null,
            [FromQuery] DateOnly? denNgay = null)
        {
            _logger.LogInformation("Getting all import orders");

            var result = ApiResponse<PagedResult<DonNhapHangDto>>.SuccessResponse(
                await _donNhapHangService.GetAllAsync(
                    pageNumber,
                    pageSize,
                    idChiNhanh,
                    idNhaCungCap,
                    tuNgay,
                    denNgay));

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật đơn nhập hàng - WAREHOUSE_STAFF
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonNhapHang(int id, [FromBody] UpdateDonNhapHangDto dto)
        {
            _logger.LogInformation($"Updating import order with id: {id}");

            await _donNhapHangService.UpdateAsync(id, dto);

            return NoContent();
        }

        /// <summary>
        /// Xóa đơn nhập hàng - ADMIN, MANAGER
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteDonNhapHang(int id)
        {
            _logger.LogInformation($"Deleting import order with id: {id}");

            await _donNhapHangService.DeleteAsync(id);

            var result = ApiResponse<string>.SuccessResponse("Đơn nhập hàng đã được xóa thành công.");
            return Ok(result);
        }
    }
}