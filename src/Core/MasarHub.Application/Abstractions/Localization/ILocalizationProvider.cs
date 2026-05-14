using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Localization
{
    public interface ILocalizationProvider : IScopedService
    {
        Task<Dictionary<string, string>> LoadAsync(string culture, CancellationToken ct = default);
    }
}
