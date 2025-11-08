using quanlybanthuoc.Dtos.DanhMuc;

namespace quanlybanthuoc.Services
{
    public interface IDanhMucService
    {
        Task<DanhMucDto> CreateAsync(CreateDanhMucDto dto);
        Task UpdateAsync(int id, UpdateDanhMucDto dto);
        Task<DanhMucDto?> GetByIdAsync(int id);
        Task<IEnumerable<DanhMucDto>> GetAllAsync();
        Task DeleteAsync(int id);
    }
}
