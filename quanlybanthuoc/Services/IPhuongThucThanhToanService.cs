using quanlybanthuoc.Dtos.PhuongThucThanhToan;

namespace quanlybanthuoc.Services
{
    public interface IPhuongThucThanhToanService
    {
        Task<PhuongThucThanhToanDto> CreateAsync(CreatePhuongThucThanhToanDto dto);
        Task UpdateAsync(int id, UpdatePhuongThucThanhToanDto dto);
        Task<PhuongThucThanhToanDto?> GetByIdAsync(int id);
        Task<IEnumerable<PhuongThucThanhToanDto>> GetAllAsync();
        Task DeleteAsync(int id);
    }
}