using quanlybanthuoc.Dtos;
using quanlybanthuoc.Dtos.LoHang;

namespace quanlybanthuoc.Services
{
    public interface ILoHangService
    {
        Task<LoHangDto?> GetByIdAsync(int id);
        Task<PagedResult<LoHangDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            int? idThuoc = null,
            int? idChiNhanh = null,
            bool? sapHetHan = null,
            int? daysToExpire = 30);
        Task<IEnumerable<LoHangDto>> GetByThuocIdAsync(int thuocId);
        Task<IEnumerable<LoHangDto>> GetLoHangSapHetHanAsync(int days, int? idChiNhanh = null);
        Task UpdateAsync(int id, UpdateLoHangDto dto);
    }
}