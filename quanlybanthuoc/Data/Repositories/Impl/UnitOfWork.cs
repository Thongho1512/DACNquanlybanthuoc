using Microsoft.EntityFrameworkCore.Storage;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ShopDbContext _context;
        private IDbContextTransaction? _transaction;
        private NguoiDungRepository? _nguoiDungRepository;
        private RefreshTokenRepository? _refreshTokenRepository;
        private VaiTroRepository? _vaiTroRepository;
        private ThuocRepository? _thuocRepository;
        private DanhMucRepository? _danhMucRepository;
        private NhaCungCapRepository? _nhaCungCapRepository;
        private ChiNhanhRepository? _chiNhanhRepository;
        private KhachHangRepository? _khachHangRepository;

        public UnitOfWork(ShopDbContext context)
        {
            _context = context;
        }

        public IDanhMucRepository DanhMucRepository =>
            _danhMucRepository ??= new DanhMucRepository(_context);

        public INhaCungCapRepository NhaCungCapRepository =>
            _nhaCungCapRepository ??= new NhaCungCapRepository(_context);

        public IChiNhanhRepository ChiNhanhRepository =>
            _chiNhanhRepository ??= new ChiNhanhRepository(_context);

        public IKhachHangRepository KhachHangRepository =>
            _khachHangRepository ??= new KhachHangRepository(_context);

        public IVaiTroRepository VaiTroRepository =>
            _vaiTroRepository ??= new VaiTroRepository(_context);

        public IRefreshTokenRepository RefreshTokenRepository =>
            _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public INguoiDungRepository NguoiDungRepository =>
            _nguoiDungRepository ??= new NguoiDungRepository(_context);

        // ✅ THÊM MỚI
        public IThuocRepository ThuocRepository =>
            _thuocRepository ??= new ThuocRepository(_context);

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