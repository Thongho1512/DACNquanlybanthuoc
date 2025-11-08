using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.PhuongThucThanhToan;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/phuongthucthanhtoan")]
    [ApiController]
    [Authorize]
    public class PhuongThucThanhToansController : ControllerBase
    {
        private readonly ILogger<PhuongThucThanhToansController> _logger;
        private readonly IPhuongThucThanhToanService _phuongThucThanhToanService;

        public PhuongThucThanhToansController(
            ILogger<PhuongThucThanhToansController> logger,
            IPhuongThucThanhToanService phuongThucThanhToanService)
        {
            _logger = logger;
            _phuongThucThanhToanService = phuongThucThanhToanService;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreatePhuongThucThanhToan([FromBody] CreatePhuongThucThanhToanDto dto)
        {
            _logger.LogInformation("Creating new payment method");
            var result = ApiResponse<PhuongThucThanhToanDto>.SuccessResponse(
                await _phuongThucThanhToanService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetPhuongThucThanhToanById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetPhuongThucThanhToanById(int id)
        {
            _logger.LogInformation($"Getting payment method by id: {id}");
            var result = ApiResponse<PhuongThucThanhToanDto>.SuccessResponse(
                await _phuongThucThanhToanService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        public async Task<IActionResult> GetAllPhuongThucThanhToans()
        {
            _logger.LogInformation("Getting all payment methods");
            var result = ApiResponse<IEnumerable<PhuongThucThanhToanDto>>.SuccessResponse(
                await _phuongThucThanhToanService.GetAllAsync());
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeletePhuongThucThanhToan(int id)
        {
            _logger.LogInformation($"Deleting payment method with id: {id}");
            await _phuongThucThanhToanService.DeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Phương thức thanh toán đã được xóa thành công.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdatePhuongThucThanhToan(int id, [FromBody] UpdatePhuongThucThanhToanDto dto)
        {
            _logger.LogInformation($"Updating payment method with id: {id}");
            await _phuongThucThanhToanService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}