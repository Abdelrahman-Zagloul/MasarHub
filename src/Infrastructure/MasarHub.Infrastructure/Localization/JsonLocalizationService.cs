using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace MasarHub.Infrastructure.Localization;

public sealed class JsonLocalizationService : ILocalizationService
{
    private readonly ICacheService _cache;
    private readonly LocalizationSettings _options;
    private readonly ILogger<JsonLocalizationService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    // prevent cache stampede per culture
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public JsonLocalizationService(
        ICacheService cache,
        IOptions<LocalizationSettings> options,
        ILogger<JsonLocalizationService> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetAsync(string key, Dictionary<string, object?>? metadata, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        var culture = CultureInfo.CurrentUICulture?.Name ?? _options.DefaultCulture;

        var value = await FindWithFallbackAsync(culture, key, ct);

        var final = value ?? key;


        // replace tokens with localization support
        if (metadata is not null)
        {
            if (metadata.TryGetValue("PropertyName", out var propertyName))
            {
                var localizedPropertyName = await FindWithFallbackAsync(culture, propertyName?.ToString()!, ct);
                metadata["PropertyName"] = localizedPropertyName ?? propertyName;
            }
            final = ReplaceTokens(final, metadata);
        }
        return final;
    }
    private async Task<string?> FindWithFallbackAsync(string culture, string key, CancellationToken ct)
    {

        var value = await FindValueAsync(culture, key, ct);
        if (value is not null)
            return value;

        try
        {
            var parent = CultureInfo.GetCultureInfo(culture).Parent.Name;
            if (!string.IsNullOrWhiteSpace(parent))
            {
                value = await FindValueAsync(parent, key, ct);
                if (value is not null)
                    return value;
            }
        }
        catch { }

        // default
        return await FindValueAsync(_options.DefaultCulture, key, ct);
    }
    private async Task<string?> FindValueAsync(string culture, string key, CancellationToken ct)
    {
        var dict = await GetCultureDictionaryAsync(culture, ct);
        return dict.TryGetValue(key, out var v) ? v : null;
    }
    private async Task<Dictionary<string, string>> GetCultureDictionaryAsync(string culture, CancellationToken ct)
    {
        var normalized = string.IsNullOrWhiteSpace(culture)
            ? _options.DefaultCulture
            : culture.Trim();

        var cacheKey = $"loc:dict:{normalized}";

        // try cache
        var cached = await _cache.GetAsync<Dictionary<string, string>>(cacheKey, ct);
        if (cached is not null)
            return cached;

        // lock per culture
        var locker = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

        await locker.WaitAsync(ct);
        try
        {
            // double check after lock
            cached = await _cache.GetAsync<Dictionary<string, string>>(cacheKey, ct);
            if (cached is not null)
                return cached;

            var loaded = await LoadFromFilesAsync(normalized, ct);

            if (loaded.Count > 0)
            {
                await _cache.SetAsync(cacheKey, loaded, TimeSpan.FromHours(_options.CacheExpirationInHours), ct);
            }

            return loaded;
        }
        finally
        {
            locker.Release();
        }
    }
    private async Task<Dictionary<string, string>> LoadFromFilesAsync(string culture, CancellationToken ct)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, _options.ResourcesPath, culture);

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!Directory.Exists(folder))
            return dict;

        var files = Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file, ct);

                if (string.IsNullOrWhiteSpace(json))
                    continue;

                var part = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions);

                if (part is null)
                    continue;

                foreach (var kv in part)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key))
                        continue;

                    if (dict.ContainsKey(kv.Key))
                    {
                        _logger.LogWarning("Duplicate localization key {Key} in {File}", kv.Key, file);
                        continue;
                    }

                    dict[kv.Key] = kv.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load localization file {File} for culture {Culture}", file, culture);
            }
        }

        return dict;
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