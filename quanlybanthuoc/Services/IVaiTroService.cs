using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.VaiTro;

namespace quanlybanthuoc.Services
{
    public interface IVaiTroService
    {
        Task<VaiTroDto> CreateAsync(CreateVaiTroDto dto);
        Task UpdateAsync(int id, UpdateVaiTroDto dto);
        Task<VaiTroDto?> GetByIdAsync(int id);
        Task<PagedResult<VaiTroDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task<IEnumerable<VaiTroDto>> GetAllActiveAsync();
        Task SoftDeleteAsync(int id);
    }
}