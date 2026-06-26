using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public class StripeSettings
    {
        [Required]
        public string SecretKey { get; set; } = null!;

        [Required]
        public string WebhookSecret { get; set; } = null!;

        [Required]
        public string SuccessUrl { get; set; } = null!;

        [Required]
        public string CancelUrl { get; set; } = null!;
    }
}
