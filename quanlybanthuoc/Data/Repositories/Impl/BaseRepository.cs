using Microsoft.EntityFrameworkCore;

namespace quanlybanthuoc.Data.Repositories.Impl
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ShopDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ShopDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task UpdateAsync(T entity)
        {
            // Mark only the root entity as Modified to avoid attaching the whole graph.
            // Attaching the full graph (DbSet.Update) can cause "already being tracked" conflicts
            // when other instances of related entities are already tracked in the DbContext.
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
