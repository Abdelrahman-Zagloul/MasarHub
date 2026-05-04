using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class JWTSettings
    {
        [Required, MinLength(32), MaxLength(128)]
        public string SecretKey { get; set; } = null!;


        [Required, MinLength(3), MaxLength(100)]
        public string Issuer { get; set; } = null!;


        [Required, MinLength(3), MaxLength(100)]
        public string Audience { get; set; } = null!;


        [Range(1, 1440)]
        public int DurationInMinutes { get; set; }
    }
}
