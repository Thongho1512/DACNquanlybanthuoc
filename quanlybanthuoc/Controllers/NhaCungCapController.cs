using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NhaCungCap;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/nhacungcaps")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class NhaCungCapsController : ControllerBase
    {
        private readonly ILogger<NhaCungCapsController> _logger;
        private readonly INhaCungCapService _nhaCungCapService;

        public NhaCungCapsController(ILogger<NhaCungCapsController> logger, INhaCungCapService nhaCungCapService)
        {
            _logger = logger;
            _nhaCungCapService = nhaCungCapService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNhaCungCap([FromBody] CreateNhaCungCapDto dto)
        {
            _logger.LogInformation("Creating new supplier");
            var result = ApiResponse<NhaCungCapDto>.SuccessResponse(await _nhaCungCapService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetNhaCungCapById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNhaCungCapById(int id)
        {
            _logger.LogInformation($"Getting supplier by id: {id}");
            var result = ApiResponse<NhaCungCapDto>.SuccessResponse(await _nhaCungCapService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNhaCungCaps(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all suppliers");
            var result = ApiResponse<PagedResult<NhaCungCapDto>>.SuccessResponse(
                await _nhaCungCapService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhaCungCap(int id)
        {
            _logger.LogInformation($"Deleting supplier with id: {id}");
            await _nhaCungCapService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Supplier deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNhaCungCap(int id, [FromBody] UpdateNhaCungCapDto dto)
        {
            _logger.LogInformation($"Updating supplier with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _nhaCungCapService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}