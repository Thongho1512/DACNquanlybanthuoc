using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.LoHang;
using quanlybanthuoc.Services;
using quanlybanthuoc.Services.Impl;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/lohangs")]
    [ApiController]
    [Authorize(Policy = "WarehouseStaff")] // Admin, Manager, Warehouse Staff
    public class LoHangsController : ControllerBase
    {
        private readonly ILogger<LoHangsController> _logger;
        private readonly ILoHangService _loHangService;

        public LoHangsController(
            ILogger<LoHangsController> logger,
            ILoHangService loHangService)
        {
            _logger = logger;
            _loHangService = loHangService;
        }

        /// <summary>
        /// Xem chi tiết lô hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoHangById(int id)
        {
            _logger.LogInformation($"Getting batch by id: {id}");
            var result = ApiResponse<LoHangDto>.SuccessResponse(
                await _loHangService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Xem danh sách lô hàng - WAREHOUSE_STAFF
        /// Theo tài liệu: Nhân viên kho theo dõi hạn sử dụng thuốc
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLoHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? idThuoc = null,
            [FromQuery] int? idChiNhanh = null,
            [FromQuery] bool? sapHetHan = null,
            [FromQuery] int? daysToExpire = 30)
        {
            _logger.LogInformation("Getting all batches");

            var result = ApiResponse<PagedResult<LoHangDto>>.SuccessResponse(
                await _loHangService.GetAllAsync(
                    pageNumber,
                    pageSize,
                    idThuoc,
                    idChiNhanh,
                    sapHetHan,
                    daysToExpire));

            return Ok(result);
        }

        /// <summary>
        /// Xem lô hàng sắp hết hạn - WAREHOUSE_STAFF
        /// Theo tài liệu: Cảnh báo thuốc hết hạn
        /// </summary>
        [HttpGet("sap-het-han")]
        public async Task<IActionResult> GetLoHangSapHetHan(
            [FromQuery] int days = 30,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting batches expiring in {days} days");

            var result = ApiResponse<IEnumerable<LoHangDto>>.SuccessResponse(
                await _loHangService.GetLoHangSapHetHanAsync(days, idChiNhanh));

            return Ok(result);
        }

        /// <summary>
        /// Xem lô hàng theo thuốc - WAREHOUSE_STAFF
        /// </summary>
        [HttpGet("thuoc/{thuocId}")]
        public async Task<IActionResult> GetLoHangByThuocId(int thuocId)
        {
            _logger.LogInformation($"Getting batches by medicine id: {thuocId}");

            var result = ApiResponse<IEnumerable<LoHangDto>>.SuccessResponse(
                await _loHangService.GetByThuocIdAsync(thuocId));

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật lô hàng - WAREHOUSE_STAFF
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoHang(int id, [FromBody] UpdateLoHangDto dto)
        {
            _logger.LogInformation($"Updating batch with id: {id}");

            await _loHangService.UpdateAsync(id, dto);

            return NoContent();
        }

        /// <summary>
        /// Tạo lô hàng thủ công - WAREHOUSE_STAFF
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLoHang(
            [FromBody] CreateLoHangDto dto,
            [FromQuery] int idChiNhanh)
        {
            _logger.LogInformation("Creating new batch manually");

            var result = ApiResponse<LoHangDto>.SuccessResponse(
                await _loHangService.CreateAsync(dto, idChiNhanh));

            return CreatedAtAction(nameof(GetLoHangById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Xóa lô hàng - ADMIN, MANAGER
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteLoHang(int id)
        {
            _logger.LogInformation($"Deleting batch with id: {id}");

            await _loHangService.DeleteAsync(id);

            var result = ApiResponse<string>.SuccessResponse("Lô hàng đã được xóa thành công.");
            return Ok(result);
        }
    }
}