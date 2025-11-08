using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhachHang;

namespace quanlybanthuoc.Services
{
    public interface IKhachHangService
    {
        Task<KhachHangDto> CreateAsync(CreateKhachHangDto dto);
        Task UpdateAsync(int id, UpdateKhachHangDto dto);
        Task<KhachHangDto?> GetByIdAsync(int id);
        Task<KhachHangDto?> GetBySdtAsync(string sdt);
        Task<PagedResult<KhachHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            bool active,
            string? searchTerm = null);
        Task SoftDeleteAsync(int id);
        Task<int> UpdateDiemTichLuyAsync(int khachHangId, int diemCong, int diemTru);
    }
}