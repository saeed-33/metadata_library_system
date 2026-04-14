using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibrarySystem.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // لا نحتاج لفحص IsDeleted هنا! 
            // لأن زميلك وضع Query Filter في ApplicationDbContext
           
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes) query = query.Include(include);
            return await query.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity)
        {
            // هنا نكتفي بالمسح العادي 
            // لأن DbContext سيمسك الحالة ويحولها لـ Soft Delete تلقائياً
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}