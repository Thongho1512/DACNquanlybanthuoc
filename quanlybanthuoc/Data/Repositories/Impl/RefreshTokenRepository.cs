using Microsoft.EntityFrameworkCore;
using quanlybanthuoc.Data.Entities;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ShopDbContext context) :base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var refreshToken = await _dbSet.Include(refreshToken => refreshToken.NguoiDung).FirstOrDefaultAsync(refreshToken => refreshToken.Token == token);
            return refreshToken;
        }
    }
}
