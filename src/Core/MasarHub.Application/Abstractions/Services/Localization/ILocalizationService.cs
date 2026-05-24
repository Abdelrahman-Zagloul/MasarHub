using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Services.Localization
{
    public interface ILocalizationService : IScopedService
    {
        Task<string> GetAsync(string key, Dictionary<string, object?>? metadata = null, CancellationToken ct = default);
    }
}
