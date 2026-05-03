using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface ICacheService : IScopedService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
    }
}
