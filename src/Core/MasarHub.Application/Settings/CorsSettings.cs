using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class CorsSettings
    {
        public const string DevelopmentPolicy = "DevelopmentPolicy";
        public const string ProductionPolicy = "ProductionPolicy";

        [Required]
        public string[] AllowedOrigins { get; set; } = [];

        [Required]
        public string[] AllowedMethods { get; set; } = [];

        [Required]
        public string[] AllowedHeaders { get; set; } = [];

        [Required]
        public bool AllowCredentials { get; set; }
    }
}
