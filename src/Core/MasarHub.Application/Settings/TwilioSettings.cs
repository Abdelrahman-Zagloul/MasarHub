using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class TwilioSettings
    {
        [Required]
        public string AccountSID { get; set; } = null!;

        [Required]
        public string AuthToken { get; set; } = null!;

        [Required, Length(10, 15)]
        public string TwilioPhoneNumber { get; set; } = null!;
    }
}
