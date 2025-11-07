using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NguoiDung;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/nguoidungs")]
    [ApiController]
    [Authorize(Roles ="ADMIN")]
    public class NguoiDungsController : ControllerBase
    {
        private readonly ILogger<NguoiDungsController> _logger;
        private readonly INguoiDungService _nguoiDungService;
        public NguoiDungsController(ILogger<NguoiDungsController> logger, INguoiDungService nguoiDungService)
        {
            _logger = logger;
            _nguoiDungService = nguoiDungService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNguoiDung([FromBody] CreateNguoiDungDto dto)
        {
            _logger.LogInformation("Creating new NguoiDung");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(await _nguoiDungService.createAsync(dto));

            return CreatedAtAction(nameof(GetNguoiDungById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNguoiDungById(int id)
        {
            _logger.LogInformation($"Getting NguoiDung by id: {id}");
            var result = ApiResponse<NguoiDungDto>.SuccessResponse(await _nguoiDungService.getByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNguoiDungsIsActive(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all NguoiDungs");
            var result = ApiResponse<PagedResult<NguoiDungDto>>.SuccessResponse(await _nguoiDungService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNguoiDung(int id)
        {
            _logger.LogInformation($"Deleting NguoiDung with id: {id}");
            await _nguoiDungService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("NguoiDung deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNguoiDung(int id, [FromBody] UpdateNguoiDungDto dto)
        {
            _logger.LogInformation($"Updating NguoiDung with id: {id}");
            if (dto == null) { 
                throw new ArgumentNullException(nameof(dto));
            }

            await _nguoiDungService.updateAsync(id, dto);

            return NoContent();
        }
    }
}
