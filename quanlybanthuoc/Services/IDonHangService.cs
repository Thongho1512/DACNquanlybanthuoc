using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonHang;

namespace quanlybanthuoc.Services
{
    public interface IDonHangService
    {
        Task<DonHangDto> CreateAsync(CreateDonHangDto dto, int idNguoiDung);
        Task<DonHangDto> CreateCustomerOrderAsync(CreateCustomerOrderDto dto, int? idKhachHang = null);
        Task<DonHangDto?> GetByIdAsync(int id);
        Task<PagedResult<DonHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idKhachHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);
        Task DeleteAsync(int id);
        Task<IEnumerable<DonHangDto>> GetByKhachHangIdAsync(int khachHangId);
        Task UpdateAsync(int id, UpdateDonHangDto dto);
    }
}