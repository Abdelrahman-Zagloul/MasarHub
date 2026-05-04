using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Persistence
{
    public interface IUnitOfWork : IScopedService
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
