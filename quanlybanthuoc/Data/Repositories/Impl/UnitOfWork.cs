using Microsoft.EntityFrameworkCore.Storage;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ShopDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly ILoggerFactory _loggerFactory;
        private NguoiDungRepository? _nguoiDungRepository;
        private RefreshTokenRepository? _refreshTokenRepository;
        private VaiTroRepository? _vaiTroRepository;
        private ThuocRepository? _thuocRepository;
        private DanhMucRepository? _danhMucRepository;
        private NhaCungCapRepository? _nhaCungCapRepository;
        private ChiNhanhRepository? _chiNhanhRepository;
        private KhachHangRepository? _khachHangRepository;
        private DonHangRepository? _donHangRepository;
        private ChiTietDonHangRepository? _chiTietDonHangRepository;
        private PhuongThucThanhToanRepository? _phuongThucThanhToanRepository;
        private LichSuDiemRepository? _lichSuDiemRepository;
        private LoHangRepository? _loHangRepository;
        private KhoHangRepository? _khoHangRepository;
        private DonNhapHangRepository? _donNhapHangRepository;
        private BaoCaoRepository? _baoCaoRepository;
        private DonGiaoHangRepository? _donGiaoHangRepository;


        public UnitOfWork(ShopDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _loggerFactory = loggerFactory;
        }
        public IBaoCaoRepository BaoCaoRepository =>
            _baoCaoRepository ??= new BaoCaoRepository(_context, _loggerFactory.CreateLogger<BaoCaoRepository>());
        public IDonGiaoHangRepository DonGiaoHangRepository =>
            _donGiaoHangRepository ??= new DonGiaoHangRepository(_context);
        public IDonNhapHangRepository DonNhapHangRepository =>
            _donNhapHangRepository ??= new DonNhapHangRepository(_context);
        public IKhoHangRepository KhoHangRepository =>
            _khoHangRepository ??= new KhoHangRepository(_context);
        public ILoHangRepository LoHangRepository =>
            _loHangRepository ??= new LoHangRepository(_context);

        public IPhuongThucThanhToanRepository PhuongThucThanhToanRepository =>
            _phuongThucThanhToanRepository ??= new PhuongThucThanhToanRepository(_context);

        public ILichSuDiemRepository LichSuDiemRepository =>
            _lichSuDiemRepository ??= new LichSuDiemRepository(_context);
        public IDonHangRepository DonHangRepository =>
            _donHangRepository ??= new DonHangRepository(_context);

        public IChiTietDonHangRepository ChiTietDonHangRepository =>
            _chiTietDonHangRepository ??= new ChiTietDonHangRepository(_context);
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

        //  THÊM MỚI
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