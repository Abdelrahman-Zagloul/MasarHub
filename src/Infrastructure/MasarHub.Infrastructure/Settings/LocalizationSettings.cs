namespace MasarHub.Application.Settings
{
    public sealed class LocalizationSettings
    {
        public List<string> SupportedCultures { get; set; } = new();
        public string DefaultCulture { get; set; } = null!;
        public string ResourcesPath { get; set; } = null!;
        public int CacheExpirationInHours { get; set; }
    }
}
