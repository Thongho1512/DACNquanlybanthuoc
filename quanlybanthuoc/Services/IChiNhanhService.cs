using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.ChiNhanh;

namespace quanlybanthuoc.Services
{
    public interface IChiNhanhService
    {
        Task<ChiNhanhDto> CreateAsync(CreateChiNhanhDto dto);
        Task UpdateAsync(int id, UpdateChiNhanhDto dto);
        Task<ChiNhanhDto?> GetByIdAsync(int id);
        Task<PagedResult<ChiNhanhDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(int id);
    }
}