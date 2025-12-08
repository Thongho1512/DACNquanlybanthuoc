using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.DonGiaoHang;

namespace quanlybanthuoc.Services
{
    public interface IDonGiaoHangService
    {
        Task<DonGiaoHangDto> CreateAsync(CreateDonGiaoHangDto dto);
        Task<DonGiaoHangDto?> GetByIdAsync(int id);
        Task<PagedResult<DonGiaoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNguoiGiaoHang = null,
            string? trangThaiGiaoHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);
        Task<DonGiaoHangDto> AssignDeliveryPersonAsync(int id, AssignDeliveryPersonDto dto);
        Task<DonGiaoHangDto> UpdateStatusAsync(int id, UpdateDeliveryStatusDto dto);
        Task<DonGiaoHangDto> CancelAsync(int id, CancelDeliveryDto dto);
        Task<IEnumerable<DonGiaoHangDto>> GetByNguoiGiaoHangIdAsync(int idNguoiGiaoHang);
    }
}

