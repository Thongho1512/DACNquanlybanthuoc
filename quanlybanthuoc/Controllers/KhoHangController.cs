using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhoHang;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/khohangs")]
    [ApiController]
    [Authorize(Policy = "WarehouseStaff")] // Admin, Manager, Warehouse Staff
    public class KhoHangsController : ControllerBase
    {
        private readonly ILogger<KhoHangsController> _logger;
        private readonly IKhoHangService _khoHangService;

        public KhoHangsController(
            ILogger<KhoHangsController> logger,
            IKhoHangService khoHangService)
        {
            _logger = logger;
            _khoHangService = khoHangService;
        }

        /// <summary>
        /// Xem chi tiết kho hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKhoHangById(int id)
        {
            _logger.LogInformation($"Getting warehouse stock by id: {id}");
            var result = ApiResponse<KhoHangDto>.SuccessResponse(
                await _khoHangService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách tồn kho - WAREHOUSE_STAFF
        /// Theo tài liệu: Nhân viên kho kiểm kê định kỳ
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllKhoHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] bool? tonKhoThap = null)
        {
            _logger.LogInformation("Getting all warehouse stocks");

            var result = ApiResponse<PagedResult<KhoHangDto>>.SuccessResponse(
                await _khoHangService.GetAllAsync(pageNumber, pageSize, idChiNhanh, tonKhoThap));

            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách tồn kho thấp - WAREHOUSE_STAFF
        /// Theo tài liệu: Cảnh báo tồn kho thấp
        /// </summary>
        [HttpGet("ton-kho-thap")]
        public async Task<IActionResult> GetTonKhoThap([FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting low stock items");

            var result = ApiResponse<IEnumerable<KhoHangDto>>.SuccessResponse(
                await _khoHangService.GetTonKhoThapAsync(idChiNhanh));

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật tồn kho tối thiểu - WAREHOUSE_STAFF
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhoHang(int id, [FromBody] UpdateKhoHangDto dto)
        {
            _logger.LogInformation($"Updating warehouse stock with id: {id}");

            await _khoHangService.UpdateAsync(id, dto);

            return NoContent();
        }

        /// <summary>
        /// Tạo kho hàng thủ công - WAREHOUSE_STAFF
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateKhoHang([FromBody] CreateKhoHangDto dto)
        {
            _logger.LogInformation("Creating new warehouse stock");

            var result = ApiResponse<KhoHangDto>.SuccessResponse(
                await _khoHangService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetKhoHangById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Xóa kho hàng - ADMIN, MANAGER
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteKhoHang(int id)
        {
            _logger.LogInformation($"Deleting warehouse stock with id: {id}");

            await _khoHangService.DeleteAsync(id);

            var result = ApiResponse<string>.SuccessResponse("Kho hàng đã được xóa thành công.");
            return Ok(result);
        }
    }
}
