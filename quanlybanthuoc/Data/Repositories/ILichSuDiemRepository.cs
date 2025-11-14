using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface ILichSuDiemRepository : IBaseRepository<LichSuDiem>
    {
        Task<IEnumerable<LichSuDiem>> GetByKhachHangIdAsync(int khachHangId);
        Task<LichSuDiem?> GetByDonHangIdAsync(int donHangId);
        Task DeleteAsync(LichSuDiem entity);
    }
}