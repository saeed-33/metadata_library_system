using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibrarySystem.DataAccess.Repositories
{
    public interface ISyncGeneratedKeys
    {
        void SyncGeneratedKeys();
    }

    public class MappedRepository<TDomain, TPersistence> : IGenericRepository<TDomain>, ISyncGeneratedKeys
        where TDomain : class
        where TPersistence : class
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<TPersistence> _dbSet;
        private readonly List<(TDomain Domain, TPersistence Persistence)> _pendingAdds = new();

        public MappedRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = context.Set<TPersistence>();
        }

        public async Task<TDomain?> GetByIdAsync(int id)
        {
            var model = await _dbSet.FindAsync(id);
            return model == null ? null : _mapper.Map<TDomain>(model);
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var models = await _dbSet.ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(models);
        }

        public async Task<IEnumerable<TDomain>> FindAsync(
            Expression<Func<TDomain, bool>> predicate,
            params Expression<Func<TDomain, object>>[] includes)
        {
            var models = await _dbSet.ToListAsync();
            var domains = _mapper.Map<IEnumerable<TDomain>>(models);
            return domains.Where(predicate.Compile()).ToList();
        }

        public async Task AddAsync(TDomain entity)
        {
            var model = _mapper.Map<TPersistence>(entity);
            await _dbSet.AddAsync(model);
            _pendingAdds.Add((entity, model));
        }

        public void Update(TDomain entity)
        {
            var model = _mapper.Map<TPersistence>(entity);
            _dbSet.Update(model);
        }

        public void Delete(TDomain entity)
        {
            var model = _mapper.Map<TPersistence>(entity);
            _dbSet.Remove(model);
        }

        public void SyncGeneratedKeys()
        {
            foreach (var (domain, persistence) in _pendingAdds)
            {
                _mapper.Map(persistence, domain);
            }

            _pendingAdds.Clear();
        }
    }
}
