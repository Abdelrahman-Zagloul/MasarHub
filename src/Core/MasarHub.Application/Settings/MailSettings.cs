using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class MailSettings
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;


        [Required, MinLength(3), MaxLength(255)]
        public string DisplayName { get; set; } = null!;


        [Required, MinLength(6), MaxLength(255)]
        public string Password { get; set; } = null!;


        [Required, MinLength(1), MaxLength(255)]
        public string Host { get; set; } = null!;

        [Range(1, 65535)]
        public int Port { get; set; }
    }
}
