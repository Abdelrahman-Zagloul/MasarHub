using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface ICacheService : IScopedService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
    }
}
