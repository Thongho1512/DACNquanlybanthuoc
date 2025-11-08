using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/nguoidungs")]
    [ApiController]
    public class NguoiDungsController : ControllerBase
    {
        private readonly ILogger<NguoiDungsController> _logger;
        private readonly INguoiDungService _nguoiDungService;

        public NguoiDungsController(ILogger<NguoiDungsController> logger, INguoiDungService nguoiDungService)
        {
            _logger = logger;
            _nguoiDungService = nguoiDungService;
        }

        /// <summary>
        /// UC41: Tạo người dùng - CHỈ ADMIN
        /// Theo tài liệu: Admin có quyền quản lý toàn bộ người dùng
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateNguoiDung([FromBody] CreateNguoiDungDto dto)
        {
            _logger.LogInformation("Creating new NguoiDung by Admin");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(await _nguoiDungService.createAsync(dto));

            return CreatedAtAction(nameof(GetNguoiDungById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// UC31: Xem danh sách người dùng - CHỈ ADMIN
        /// Theo tài liệu: Chỉ Admin mới được xem danh sách tất cả người dùng
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllNguoiDungsIsActive(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all NguoiDungs - Admin only");
            var result = ApiResponse<PagedResult<NguoiDungDto>>.SuccessResponse(
                await _nguoiDungService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        /// <summary>
        /// Xem chi tiết người dùng theo ID - CHỈ ADMIN
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetNguoiDungById(int id)
        {
            _logger.LogInformation($"Getting NguoiDung by id: {id} - Admin only");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(
                await _nguoiDungService.getByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// UC22: Cập nhật người dùng - CHỈ ADMIN
        /// Theo tài liệu: Admin cập nhật thông tin và quyền hạn tài khoản người dùng
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateNguoiDung(int id, [FromBody] UpdateNguoiDungDto dto)
        {
            _logger.LogInformation($"Updating NguoiDung with id: {id} - Admin only");

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _nguoiDungService.updateAsync(id, dto);

            return NoContent();
        }

        /// <summary>
        /// UC23: Xóa người dùng (Soft Delete) - CHỈ ADMIN
        /// Theo tài liệu: Admin vô hiệu hóa tài khoản người dùng
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteNguoiDung(int id)
        {
            _logger.LogInformation($"Deleting NguoiDung with id: {id} - Admin only");
            await _nguoiDungService.SoftDeleteAsync(id);

            var result = ApiResponse<string>.SuccessResponse("NguoiDung deleted successfully");
            return Ok(result);
        }

        /// <summary>
        /// UC32: Tìm kiếm người dùng - CHỈ ADMIN
        /// </summary>
        [HttpGet("search")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SearchNguoiDung(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation($"Searching NguoiDung with term: {searchTerm} - Admin only");

            var result = ApiResponse<PagedResult<NguoiDungDto>>.SuccessResponse(
                await _nguoiDungService.GetAllAsync(pageNumber, pageSize, true, searchTerm));

            return Ok(result);
        }
    }
}