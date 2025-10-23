namespace quanlybanthuoc.Data.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task<T?> GetByIdAsync(int id);
    }
}
