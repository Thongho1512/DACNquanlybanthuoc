namespace quanlybanthuoc.Data.Repositories
{
    public interface IUnitOfWork
    {
        INguoiDungRepository NguoiDungRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
