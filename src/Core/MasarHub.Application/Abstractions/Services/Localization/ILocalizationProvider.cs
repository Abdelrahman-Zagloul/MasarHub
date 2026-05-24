using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Services.Localization
{
    public interface ILocalizationProvider : IScopedService
    {
        Task<Dictionary<string, string>> LoadAsync(string culture, CancellationToken ct = default);
    }
}
