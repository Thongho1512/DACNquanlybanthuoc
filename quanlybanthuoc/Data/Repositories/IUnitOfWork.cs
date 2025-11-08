namespace quanlybanthuoc.Data.Repositories
{
    public interface IUnitOfWork
    {
        INguoiDungRepository NguoiDungRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IVaiTroRepository VaiTroRepository { get; }
        IThuocRepository ThuocRepository { get; }

        IDanhMucRepository DanhMucRepository { get; }
        INhaCungCapRepository NhaCungCapRepository { get; }
        IChiNhanhRepository ChiNhanhRepository { get; }
        IKhachHangRepository KhachHangRepository { get; }
        IDonHangRepository DonHangRepository { get; }
        IChiTietDonHangRepository ChiTietDonHangRepository { get; }

        IPhuongThucThanhToanRepository PhuongThucThanhToanRepository { get; }
        ILichSuDiemRepository LichSuDiemRepository { get; }

        ILoHangRepository LoHangRepository { get; }
        IKhoHangRepository KhoHangRepository { get; }
        IDonNhapHangRepository DonNhapHangRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}