using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Localization
{
    public interface ILocalizationService : IScopedService
    {
        Task<string> GetAsync(string key, object? args = null, CancellationToken ct = default);
    }
}
