using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface ILichSuDiemRepository : IBaseRepository<LichSuDiem>
    {
        Task<IEnumerable<LichSuDiem>> GetByKhachHangIdAsync(int khachHangId);
    }
}