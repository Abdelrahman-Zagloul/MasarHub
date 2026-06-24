using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Domain.Common.Base;
using MasarHub.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MasarHub.Infrastructure.Persistence.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly MasarHubDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfRepository(MasarHubDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.FindAsync(id, ct);
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);
            return await query.FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Attach(TEntity entity)
            => _dbSet.Attach(entity);

        public void AttachRange(IEnumerable<TEntity> entities)
            => _context.AttachRange(entities);

        public void Remove(TEntity entity)
            => _dbSet.Remove(entity);

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.Where(predicate).ToListAsync(ct);


        public async Task<TEntity?> GetWithDeletedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _context.Set<TEntity>().IgnoreQueryFilters()
                .FirstOrDefaultAsync(predicate, ct);

        public async Task<List<TEntity>> GetAllWithDeletedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _context.Set<TEntity>().IgnoreQueryFilters()
                .Where(predicate).ToListAsync(ct);
    }
}
