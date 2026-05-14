using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MasarHub.Infrastructure.Localization
{
    public sealed class JsonLocalizationProvider : ILocalizationProvider
    {
        private readonly LocalizationSettings _options;
        private readonly ILogger<JsonLocalizationProvider> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public JsonLocalizationProvider(IOptions<LocalizationSettings> options, ILogger<JsonLocalizationProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> LoadAsync(string culture, CancellationToken ct = default)
        {
            var folder = Path.Combine(AppContext.BaseDirectory, _options.ResourcesPath, culture);
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!Directory.Exists(folder))
                return dictionary;

            var files = Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file, ct);
                    if (string.IsNullOrWhiteSpace(json))
                        continue;

                    var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions);
                    if (translations is null)
                        continue;

                    foreach (var kv in translations)
                    {
                        if (string.IsNullOrWhiteSpace(kv.Key))
                            continue;

                        if (!dictionary.TryAdd(kv.Key, kv.Value ?? string.Empty))
                            _logger.LogWarning("Duplicate localization key {Key} in {File}", kv.Key, file);

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load localization file {File} for culture {Culture}", file, culture);
                }
            }
            return dictionary;
        }
    }
}