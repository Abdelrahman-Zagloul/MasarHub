using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class LocalizationSettings
    {
        [Required, MinLength(1)]
        public List<string> SupportedCultures { get; set; } = new();

        [Required, MinLength(2)]
        public string DefaultCulture { get; set; } = null!;

        [Required]
        public string ResourcesPath { get; set; } = null!;

        [Range(1, 168)]
        public int CacheExpirationInHours { get; set; }
    }
}