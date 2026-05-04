using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class RefreshTokenSettings
    {
        [Range(1, 365)]
        public int RefreshTokenLifetimeDays { get; set; }


        [Required, MinLength(32), MaxLength(128)]
        public string RefreshTokenHashKey { get; set; } = null!;
    }
}
