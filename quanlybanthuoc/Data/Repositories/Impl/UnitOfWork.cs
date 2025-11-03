
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ShopDbContext _context;
        private IDbContextTransaction? _transaction;
        private NguoiDungRepository? _nguoiDungRepository;
        private RefreshTokenRepository? _refreshTokenRepository;

        public UnitOfWork(ShopDbContext context)
        {
            _context = context;
        }

        public IRefreshTokenRepository RefreshTokenRepository => 
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);
        public INguoiDungRepository NguoiDungRepository =>
            _nguoiDungRepository ??= new NguoiDungRepository(_context);

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                Dispose();
            }
        }

        public void Dispose()
        {
            _transaction?.DisposeAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                Dispose();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
