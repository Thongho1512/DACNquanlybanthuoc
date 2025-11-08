using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonNhapHang;

namespace quanlybanthuoc.Services
{
    public interface IDonNhapHangService
    {
        Task<DonNhapHangDto> CreateAsync(CreateDonNhapHangDto dto, int idNguoiNhan);
        Task<DonNhapHangDto?> GetByIdAsync(int id);
        Task<PagedResult<DonNhapHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNhaCungCap = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);
    }
}