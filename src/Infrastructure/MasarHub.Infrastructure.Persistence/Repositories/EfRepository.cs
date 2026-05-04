using MasarHub.Application.Abstractions.Persistence;
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
