using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IChiTietDonHangRepository : IBaseRepository<ChiTietDonHang>
    {
        Task<IEnumerable<ChiTietDonHang>> GetByDonHangIdAsync(int donHangId);
        Task CreateRangeAsync(IEnumerable<ChiTietDonHang> chiTietDonHangs);
        Task DeleteRangeAsync(IEnumerable<ChiTietDonHang> chiTietDonHangs);
    }
}