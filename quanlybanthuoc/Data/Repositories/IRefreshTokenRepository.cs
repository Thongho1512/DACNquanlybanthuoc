using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
