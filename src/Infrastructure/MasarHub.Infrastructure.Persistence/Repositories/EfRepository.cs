using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MasarHub.Infrastructure.Persistence.Repositories
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _dbSet.FindAsync(id, ct);
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            => await _dbSet.Where(predicate).FirstOrDefaultAsync(ct);

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
            => await _dbSet.AddAsync(entity, ct);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Remove(TEntity entity)
            => _dbSet.Remove(entity);


    }
}
