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
            var model = await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id);
            return model == null ? null : _mapper.Map<TDomain>(model);
        }

        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var models = await _dbSet.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(models);
        }

        public async Task<IEnumerable<TDomain>> FindAsync(
            Expression<Func<TDomain, bool>> predicate,
            params Expression<Func<TDomain, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                if (ExpressionTranslator.TryTranslate<TDomain, TPersistence, object>(include, out var translatedInclude))
                {
                    query = query.Include(translatedInclude);
                }
            }

            if (ExpressionTranslator.TryTranslate<TDomain, TPersistence, bool>(predicate, out var translatedPredicate))
            {
                var filteredModels = await query.Where(translatedPredicate).ToListAsync();
                return _mapper.Map<IEnumerable<TDomain>>(filteredModels);
            }

            var models = await query.ToListAsync();
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

    internal static class ExpressionTranslator
    {
        public static bool TryTranslate<TSource, TDestination, TResult>(
            Expression<Func<TSource, TResult>> expression,
            out Expression<Func<TDestination, TResult>> translated)
        {
            var parameter = Expression.Parameter(typeof(TDestination), expression.Parameters[0].Name);
            var visitor = new MatchingMemberVisitor(expression.Parameters[0], parameter);
            var body = visitor.Visit(expression.Body);

            if (!visitor.CanTranslate || body == null)
            {
                translated = null!;
                return false;
            }

            translated = Expression.Lambda<Func<TDestination, TResult>>(body, parameter);
            return true;
        }

        private sealed class MatchingMemberVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _sourceParameter;
            private readonly ParameterExpression _destinationParameter;

            public bool CanTranslate { get; private set; } = true;

            public MatchingMemberVisitor(ParameterExpression sourceParameter, ParameterExpression destinationParameter)
            {
                _sourceParameter = sourceParameter;
                _destinationParameter = destinationParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _sourceParameter ? _destinationParameter : node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var owner = Visit(node.Expression);
                if (!CanTranslate || owner == null)
                {
                    return node;
                }

                var member = owner.Type.GetMember(node.Member.Name).FirstOrDefault();
                if (member == null)
                {
                    CanTranslate = false;
                    return node;
                }

                return Expression.MakeMemberAccess(owner, member);
            }
        }
    }
}
