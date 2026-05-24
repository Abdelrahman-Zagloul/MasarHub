using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Repositories
{
    public interface IUnitOfWork : IScopedService
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
