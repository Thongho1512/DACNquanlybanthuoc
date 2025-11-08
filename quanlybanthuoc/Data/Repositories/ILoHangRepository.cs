using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface ILoHangRepository : IBaseRepository<LoHang>
    {
        Task<PagedResult<LoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idThuoc = null,
            int? idChiNhanh = null,
            bool? sapHetHan = null,
            int? daysToExpire = 30);

        Task<IEnumerable<LoHang>> GetByThuocIdAsync(int thuocId);
        Task<IEnumerable<LoHang>> GetByDonNhapHangIdAsync(int donNhapHangId);
        Task<LoHang?> GetBySoLoAsync(string soLo);
    }
}