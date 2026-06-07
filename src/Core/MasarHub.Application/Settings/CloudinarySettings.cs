using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class CloudinarySettings
    {
        [Required]
        public string CloudName { get; set; } = null!;

        [Required]
        public string APIKey { get; set; } = null!;

        [Required]
        public string APISecret { get; set; } = null!;
    }
}
