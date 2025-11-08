using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IPhuongThucThanhToanRepository : IBaseRepository<PhuongThucThanhToan>
    {
        Task<IEnumerable<PhuongThucThanhToan>> GetAllAsync();
    }
}