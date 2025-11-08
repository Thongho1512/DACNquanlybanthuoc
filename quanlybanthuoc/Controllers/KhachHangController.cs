using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhachHang;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/khachhangs")]
    [ApiController]
    [Authorize]
    public class KhachHangsController : ControllerBase
    {
        private readonly ILogger<KhachHangsController> _logger;
        private readonly IKhachHangService _khachHangService;

        public KhachHangsController(ILogger<KhachHangsController> logger, IKhachHangService khachHangService)
        {
            _logger = logger;
            _khachHangService = khachHangService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateKhachHang([FromBody] CreateKhachHangDto dto)
        {
            _logger.LogInformation("Creating new customer");
            var result = ApiResponse<KhachHangDto>.SuccessResponse(await _khachHangService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetKhachHangById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKhachHangById(int id)
        {
            _logger.LogInformation($"Getting customer by id: {id}");
            var result = ApiResponse<KhachHangDto>.SuccessResponse(await _khachHangService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet("phone/{sdt}")]
        public async Task<IActionResult> GetKhachHangBySdt(string sdt)
        {
            _logger.LogInformation($"Getting customer by phone: {sdt}");
            var result = ApiResponse<KhachHangDto>.SuccessResponse(await _khachHangService.GetBySdtAsync(sdt));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKhachHangs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all customers");
            var result = ApiResponse<PagedResult<KhachHangDto>>.SuccessResponse(
                await _khachHangService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteKhachHang(int id)
        {
            _logger.LogInformation($"Deleting customer with id: {id}");
            await _khachHangService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Customer deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKhachHang(int id, [FromBody] UpdateKhachHangDto dto)
        {
            _logger.LogInformation($"Updating customer with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _khachHangService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}