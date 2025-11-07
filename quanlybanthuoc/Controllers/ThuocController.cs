using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.Thuoc;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/thuocs")]
    [ApiController]
    [Authorize]
    public class ThuocsController : ControllerBase
    {
        private readonly ILogger<ThuocsController> _logger;
        private readonly IThuocService _thuocService;

        public ThuocsController(ILogger<ThuocsController> logger, IThuocService thuocService)
        {
            _logger = logger;
            _thuocService = thuocService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateThuoc([FromBody] CreateThuocDto dto)
        {
            _logger.LogInformation("Creating new medicine");
            var result = ApiResponse<ThuocDto>.SuccessResponse(await _thuocService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetThuocById), new { id = result.Data?.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetThuocById(int id)
        {
            _logger.LogInformation($"Getting medicine by id: {id}");
            var result = ApiResponse<ThuocDto>.SuccessResponse(await _thuocService.GetByIdAsync(id));
            return Ok(result);
        }

        [HttpGet]
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteThuoc(int id)
        {
            _logger.LogInformation($"Deleting medicine with id: {id}");
            await _thuocService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Medicine deleted successfully");
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
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

        [HttpGet("expiring-soon")]
        public async Task<IActionResult> GetThuocSapHetHan(
            [FromQuery] int days = 30,
            [FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation($"Getting medicines expiring in {days} days");
            var thuocs = await _thuocService.GetThuocSapHetHanAsync(days, idChiNhanh);
            var result = ApiResponse<IEnumerable<ThuocDto>>.SuccessResponse(thuocs);
            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetThuocTonKhoThap([FromQuery] int? idChiNhanh = null)
        {
            _logger.LogInformation("Getting medicines with low stock");
            var thuocs = await _thuocService.GetThuocTonKhoThapAsync(idChiNhanh);
            var result = ApiResponse<IEnumerable<ThuocDto>>.SuccessResponse(thuocs);
            return Ok(result);
        }
    }
}