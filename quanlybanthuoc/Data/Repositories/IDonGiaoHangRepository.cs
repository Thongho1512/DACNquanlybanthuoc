using quanlybanthuoc.Data.Entities;
using quanlybanthuoc.Dtos;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IDonGiaoHangRepository : IBaseRepository<DonGiaoHang>
    {
        Task<PagedResult<DonGiaoHang>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            int? idChiNhanh = null,
            int? idNguoiGiaoHang = null,
            string? trangThaiGiaoHang = null,
            DateOnly? tuNgay = null,
            DateOnly? denNgay = null);

        Task<DonGiaoHang?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<DonGiaoHang>> GetByNguoiGiaoHangIdAsync(int idNguoiGiaoHang);
        Task<IEnumerable<DonGiaoHang>> GetByDonHangIdAsync(int idDonHang);
    }
}

