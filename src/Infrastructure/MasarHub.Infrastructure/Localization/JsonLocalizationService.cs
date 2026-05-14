using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace MasarHub.Infrastructure.Localization;

public sealed class JsonLocalizationService : ILocalizationService
{
    private readonly ICacheService _cache;
    private readonly ILocalizationProvider _provider;
    private readonly LocalizationSettings _options;
    private readonly ILogger<JsonLocalizationService> _logger;

    public JsonLocalizationService(ICacheService cache, ILocalizationProvider provider, IOptions<LocalizationSettings> options, ILogger<JsonLocalizationService> logger)
    {
        _cache = cache;
        _provider = provider;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetAsync(string key, Dictionary<string, object?>? metadata, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        var culture = CultureInfo.CurrentUICulture?.Name ?? _options.DefaultCulture;

        var value = await FindValueAsync(culture, key, ct);
        var final = value ?? key;


        // replace tokens with localization
        if (metadata is not null)
        {
            if (metadata.TryGetValue("PropertyName", out var propertyName))
            {
                var localizedPropertyName = await FindValueAsync(culture, propertyName?.ToString()!, ct);
                metadata["PropertyName"] = localizedPropertyName ?? propertyName;
            }
            final = ReplaceTokens(final, metadata);
        }
        return final;
    }

    private async Task<string?> FindValueAsync(string culture, string key, CancellationToken ct)
    {
        var dict = await GetCultureDictionaryAsync(culture, ct);
        return dict.TryGetValue(key, out var value) ? value : null;
    }

    private async Task<Dictionary<string, string>> GetCultureDictionaryAsync(string culture, CancellationToken ct)
    {
        var cacheKey = $"loc:dict:{culture}";
        var cached = await _cache.GetAsync<Dictionary<string, string>>(cacheKey, ct);
        if (cached != null)
            return cached;

        _logger.LogWarning("Localization cache missing for culture {Culture}", culture);

        var loaded = await _provider.LoadAsync(culture, ct);
        if (loaded.Count > 0)
            await _cache.SetAsync(cacheKey, loaded, TimeSpan.FromHours(_options.CacheExpirationInHours), ct);

        return loaded;
    }

    private static string ReplaceTokens(string text, Dictionary<string, object?> metadata)
    {
        foreach (var item in metadata)
        {
            var token = $"{{{item.Key}}}";
            text = text.Replace(token, item.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        return text;
    }
}