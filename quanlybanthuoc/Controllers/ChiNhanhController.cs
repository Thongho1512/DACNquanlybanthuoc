using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.ChiNhanh;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/chinhanhs")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class ChiNhanhsController : ControllerBase
    {
        private readonly ILogger<ChiNhanhsController> _logger;
        private readonly IChiNhanhService _chiNhanhService;

        public ChiNhanhsController(ILogger<ChiNhanhsController> logger, IChiNhanhService chiNhanhService)
        {
            _logger = logger;
            _chiNhanhService = chiNhanhService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChiNhanh([FromBody] CreateChiNhanhDto dto)
        {
            _logger.LogInformation("Creating new branch");
            var result = ApiResponse<ChiNhanhDto>.SuccessResponse(await _chiNhanhService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetChiNhanhById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiNhanhById(int id)
        {
            _logger.LogInformation($"Getting branch by id: {id}");
            var result = ApiResponse<ChiNhanhDto>.SuccessResponse(await _chiNhanhService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllChiNhanhs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all branches");
            var result = ApiResponse<PagedResult<ChiNhanhDto>>.SuccessResponse(
                await _chiNhanhService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChiNhanh(int id)
        {
            _logger.LogInformation($"Deleting branch with id: {id}");
            await _chiNhanhService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Branch deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChiNhanh(int id, [FromBody] UpdateChiNhanhDto dto)
        {
            _logger.LogInformation($"Updating branch with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _chiNhanhService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}