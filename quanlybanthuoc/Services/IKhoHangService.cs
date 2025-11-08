using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.KhoHang;

namespace quanlybanthuoc.Services
{
    public interface IKhoHangService
    {
        Task<KhoHangDto?> GetByIdAsync(int id);
        Task<PagedResult<KhoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            bool? tonKhoThap = null);
        Task<IEnumerable<KhoHangDto>> GetTonKhoThapAsync(int? idChiNhanh = null);
        Task UpdateAsync(int id, UpdateKhoHangDto dto);
    }
}