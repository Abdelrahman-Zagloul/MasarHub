using MasarHub.Application.Common.DI;
using MasarHub.Domain.Common.Base;

namespace MasarHub.Application.Abstractions.Persistence
{
    public interface IRepository<TEntity> : IScopedService where TEntity : BaseEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}
