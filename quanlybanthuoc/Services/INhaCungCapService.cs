using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.NhaCungCap;

namespace quanlybanthuoc.Services
{
    public interface INhaCungCapService
    {
        Task<NhaCungCapDto> CreateAsync(CreateNhaCungCapDto dto);
        Task UpdateAsync(int id, UpdateNhaCungCapDto dto);
        Task<NhaCungCapDto?> GetByIdAsync(int id);
        Task<PagedResult<NhaCungCapDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(int id);
    }
}