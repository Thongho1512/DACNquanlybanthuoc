using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Services;
using System;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/thuocs")]
    [ApiController]
    [Authorize] // Tất cả endpoint yêu cầu đăng nhập
    public class ThuocsController : ControllerBase
    {
        private readonly ILogger<ThuocsController> _logger;
        private readonly IThuocService _thuocService;

        public ThuocsController(ILogger<ThuocsController> logger, IThuocService thuocService)
        {
            _logger = logger;
            _thuocService = thuocService;
        }

        /// <summary>
        /// UC04: Tạo thuốc mới - ADMIN, MANAGER, WAREHOUSE_STAFF
        /// Theo tài liệu: Quản lý thuốc - nhân viên kho nhập hàng mới
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "WarehouseStaff")]
        public async Task<IActionResult> CreateThuoc([FromBody] CreateThuocDto dto)
        {
            _logger.LogInformation("Creating new medicine");
            var result = ApiResponse<ThuocDto>.SuccessResponse(await _thuocService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetThuocById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// UC30: Xem danh sách thuốc - TẤT CẢ NGƯỜI DÙNG
        /// Theo tài liệu: UC30 - Tất cả actor có thể xem danh sách thuốc
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetAllThuocs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? idDanhMuc = null)
        {
            _logger.LogInformation("Getting all medicines");
            var result = ApiResponse<PagedResult<ThuocDto>>.SuccessResponse(
                await _thuocService.GetAllAsync(pageNumber, pageSize, active, searchTerm, idDanhMuc));
            return Ok(result);
        }

        /// <summary>
        /// Xem chi tiết thuốc theo ID - TẤT CẢ NGƯỜI DÙNG
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetThuocById(int id)
        {
            _logger.LogInformation($"Getting medicine by id: {id}");
            var result = ApiResponse<ThuocDto>.SuccessResponse(await _thuocService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// UC16: Tìm kiếm thuốc - TẤT CẢ NGƯỜI DÙNG
        /// Theo tài liệu: UC16 - Tất cả actor có thể tìm kiếm thuốc theo tên
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> SearchThuoc(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Searching medicine with term: {searchTerm}");
            var result = ApiResponse<PagedResult<ThuocDto>>.SuccessResponse(
                await _thuocService.GetAllAsync(pageNumber, pageSize, true, searchTerm, null));
            return Ok(result);
        }

        /// <summary>
        /// UC20: Cập nhật thuốc - ADMIN, MANAGER, WAREHOUSE_STAFF
        /// Theo tài liệu: UC20 - Nhân viên kho cập nhật thông tin thuốc
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "WarehouseStaff")]
        public async Task<IActionResult> UpdateThuoc(int id, [FromBody] UpdateThuocDto dto)
        {
            _logger.LogInformation($"Updating medicine with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _thuocService.UpdateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// UC21: Xóa thuốc (Soft Delete) - ADMIN, MANAGER
        /// Theo tài liệu: UC21 - Vô hiệu hóa thuốc, chỉ cấp quản lý trở lên
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> DeleteThuoc(int id)
        {
            _logger.LogInformation($"Deleting medicine with id: {id}");
            await _thuocService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Medicine deleted successfully");
            return Ok(result);
        }

        /// <summary>
        /// Xem thuốc sắp hết hạn - ADMIN, MANAGER, WAREHOUSE_STAFF
        /// Theo tài liệu: Nhân viên kho theo dõi hạn sử dụng
        /// </summary>
        [HttpGet("expiring-soon")]
        [Authorize(Policy = "WarehouseStaff")]
        public async Task<IActionResult> GetThuocSapHetHan(
            [FromQuery] int days = 30,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting medicines expiring in {days} days");
            var thuocs = await _thuocService.GetThuocSapHetHanAsync(days, idChiNhanh);
            var result = ApiResponse<IEnumerable<ThuocDto>>.SuccessResponse(thuocs);
            return Ok(result);
        }

        /// <summary>
        /// Xem thuốc tồn kho thấp - ADMIN, MANAGER, WAREHOUSE_STAFF
        /// Theo tài liệu: Nhân viên kho kiểm kê và cảnh báo tồn kho thấp
        /// </summary>
        [HttpGet("low-stock")]
        [Authorize(Policy = "WarehouseStaff")]
        public async Task<IActionResult> GetThuocTonKhoThap([FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting medicines with low stock");
            var thuocs = await _thuocService.GetThuocTonKhoThapAsync(idChiNhanh);
            var result = ApiResponse<IEnumerable<ThuocDto>>.SuccessResponse(thuocs);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách thuốc theo chi nhánh - TẤT CẢ NGƯỜI DÙNG
        /// Chỉ lấy các thuốc có tồn kho > 0 tại chi nhánh
        /// </summary>
        [HttpGet("by-branch/{idChiNhanh}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetThuocByChiNhanh(
            int idChiNhanh,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? idDanhMuc = null)
        {
            _logger.LogInformation($"Getting medicines by branch: {idChiNhanh}");

            var result = ApiResponse<PagedResult<ThuocDto>>.SuccessResponse(
                await _thuocService.GetByChiNhanhIdAsync(
                    idChiNhanh,
                    pageNumber,
                    pageSize,
                    active,
                    searchTerm,
                    idDanhMuc));

            return Ok(result);
        }
    }
}