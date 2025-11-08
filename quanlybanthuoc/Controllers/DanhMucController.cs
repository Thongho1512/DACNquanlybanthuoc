using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DanhMuc;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/danhmucs")]
    [ApiController]
    [Authorize]
    public class DanhMucsController : ControllerBase
    {
        private readonly ILogger<DanhMucsController> _logger;
        private readonly IDanhMucService _danhMucService;

        public DanhMucsController(ILogger<DanhMucsController> logger, IDanhMucService danhMucService)
        {
            _logger = logger;
            _danhMucService = danhMucService;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateDanhMuc([FromBody] CreateDanhMucDto dto)
        {
            _logger.LogInformation("Creating new category");
            var result = ApiResponse<DanhMucDto>.SuccessResponse(await _danhMucService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetDanhMucById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDanhMucById(int id)
        {
            _logger.LogInformation($"Getting category by id: {id}");
            var result = ApiResponse<DanhMucDto>.SuccessResponse(await _danhMucService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDanhMucs()
        {
            _logger.LogInformation("Getting all categories");
            var result = ApiResponse<IEnumerable<DanhMucDto>>.SuccessResponse(await _danhMucService.GetAllAsync());
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteDanhMuc(int id)
        {
            _logger.LogInformation($"Deleting category with id: {id}");
            await _danhMucService.DeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Category deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateDanhMuc(int id, [FromBody] UpdateDanhMucDto dto)
        {
            _logger.LogInformation($"Updating category with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _danhMucService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}