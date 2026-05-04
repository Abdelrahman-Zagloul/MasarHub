using MasarHub.Application.Common.DI;
using MasarHub.Domain.Common.Base;
using System.Linq.Expressions;

namespace MasarHub.Application.Abstractions.Persistence
{
    public interface IRepository<TEntity> : IScopedService where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
