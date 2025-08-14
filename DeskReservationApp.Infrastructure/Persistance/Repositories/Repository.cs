using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DeskReservationApp.Domain.Interfaces;

namespace DeskReservationApp.Infrastructure.Persistance.Repositories
{
    /// <summary>
    /// Generic repository implementation for common CRUD operations
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DeskReservationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DeskReservationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
