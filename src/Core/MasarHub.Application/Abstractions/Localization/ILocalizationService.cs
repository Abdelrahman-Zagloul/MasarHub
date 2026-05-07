using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Localization
{
    public interface ILocalizationService : IScopedService
    {
        Task<string> GetAsync(string key, Dictionary<string, object?>? metadata = null, CancellationToken ct = default);
    }
}
