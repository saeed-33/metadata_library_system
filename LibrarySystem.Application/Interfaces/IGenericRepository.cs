using System.Linq.Expressions;

namespace LibrarySystem.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // جلب سجل واحد عن طريق الـ ID
        Task<T?> GetByIdAsync(int id);

        // جلب كل السجلات
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllWithDeletedAsync();


        // البحث باستخدام شرط مع إمكانية جلب البيانات المرتبطة (Include)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindWithDeletedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        // إضافة سجل جديد
        Task AddAsync(T entity);


        // تعديل سجل
        void Update(T entity);

        // حذف (في حالتنا سيكون Soft Delete)
        void Delete(T entity);

    }
}
