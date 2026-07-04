using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public class PaymobSettings
    {
        [Required]
        public string PublicKey { get; set; } = null!;

        [Required]
        public string SecretKey { get; set; } = null!;

        [Required]
        public string HMAC { get; set; } = null!;

        [Required]
        public int IntegrationId { get; set; }
    }
}