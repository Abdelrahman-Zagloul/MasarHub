using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Common.DI;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Localization
{
    public sealed class LocalizationSyncJob : IScopedService
    {
        private readonly ICacheService _cache;
        private readonly ILocalizationProvider _provider;
        private readonly LocalizationSettings _options;
        private readonly ILogger<LocalizationSyncJob> _logger;

        public LocalizationSyncJob(ICacheService cache, ILocalizationProvider provider, IOptions<LocalizationSettings> options, ILogger<LocalizationSyncJob> logger)
        {
            _cache = cache;
            _provider = provider;
            _options = options.Value;
            _logger = logger;
        }

        public async Task SyncAsync(CancellationToken ct = default)
        {
            var root = Path.Combine(AppContext.BaseDirectory, _options.ResourcesPath);
            if (!Directory.Exists(root))
            {
                _logger.LogWarning("Localization folder not found: {Path}", root);
                return;
            }

            var cultures = Directory.GetDirectories(root);
            foreach (var culturePath in cultures)
            {
                ct.ThrowIfCancellationRequested();

                var culture = Path.GetFileName(culturePath);
                var dictionary = await _provider.LoadAsync(culture, ct);

                var cacheKey = $"loc:dict:{culture}";
                try
                {
                    await _cache.SetAsync(cacheKey, dictionary, TimeSpan.FromHours(_options.CacheExpirationInHours), ct);
                    _logger.LogInformation("Localization cache populated for culture {Culture}", culture);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to populate localization cache for culture {Culture}", culture);
                }

            }
        }
    }
}