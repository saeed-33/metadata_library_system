using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
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

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // نستخدم شرط IsDeleted إذا كان الكائن يرث من BasePersistenceModel
            if (typeof(BasePersistenceModel).IsAssignableFrom(typeof(T)))
            {
                return await _dbSet.Where(e => !((BasePersistenceModel)(object)e).IsDeleted).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }
        //params => Eager loading
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // إضافة الـ Includes (مثل جلب الـ Media مع الـ Item)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            // تعبئة بيانات التدقيق تلقائياً إذا كان الكائن يرث من BasePersistenceModel
            if (entity is BasePersistenceModel baseModel)
            {
                baseModel.CreatedAt = DateTime.Now;
                baseModel.IsDeleted = false;
            }
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            if (entity is BasePersistenceModel baseModel)
            {
                baseModel.ModifiedAt = DateTime.Now;
            }
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            // تنفيذ الـ Soft Delete بدلاً من الحذف الفعلي
            if (entity is BasePersistenceModel baseModel)
            {
                baseModel.IsDeleted = true;
                baseModel.DeletedAt = DateTime.Now;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}