using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.VaiTro;
using quanlybanthuoc.Services;

namespace quanlybanthuoc.Controllers
{
    [Route("api/v1/vaitros")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class VaiTrosController : ControllerBase
    {
        private readonly ILogger<VaiTrosController> _logger;
        private readonly IVaiTroService _vaiTroService;

        public VaiTrosController(ILogger<VaiTrosController> logger, IVaiTroService vaiTroService)
        {
            _logger = logger;
            _vaiTroService = vaiTroService;
        }

        /// <summary>
        /// Create new role
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVaiTro([FromBody] CreateVaiTroDto dto)
        {
            _logger.LogInformation("Creating new role");
            var result = ApiResponse<VaiTroDto>.SuccessResponse(await _vaiTroService.CreateAsync(dto));

            return CreatedAtAction(nameof(GetVaiTroById), new { id = result.Data?.Id }, result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVaiTroById(int id)
        {
            _logger.LogInformation($"Getting role by id: {id}");
            var result = ApiResponse<VaiTroDto>.SuccessResponse(await _vaiTroService.GetByIdAsync(id));
            return Ok(result);
        }

        /// <summary>
        /// Get all roles with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVaiTros(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool active = true,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation("Getting all roles");
            var result = ApiResponse<PagedResult<VaiTroDto>>.SuccessResponse(
                await _vaiTroService.GetAllAsync(pageNumber, pageSize, active, searchTerm));
            return Ok(result);
        }

        /// <summary>
        /// Get all active roles (for dropdown/select)
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous] // Allow access for user registration
        public async Task<IActionResult> GetAllActiveVaiTros()
        {
            _logger.LogInformation("Getting all active roles");
            var result = ApiResponse<IEnumerable<VaiTroDto>>.SuccessResponse(
                await _vaiTroService.GetAllActiveAsync());
            return Ok(result);
        }

        /// <summary>
        /// Soft delete role
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVaiTro(int id)
        {
            _logger.LogInformation($"Deleting role with id: {id}");
            await _vaiTroService.SoftDeleteAsync(id);
            var result = ApiResponse<string>.SuccessResponse("Role deleted successfully");
            return Ok(result);
        }

        /// <summary>
        /// Update role
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVaiTro(int id, [FromBody] UpdateVaiTroDto dto)
        {
            _logger.LogInformation($"Updating role with id: {id}");
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            await _vaiTroService.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}