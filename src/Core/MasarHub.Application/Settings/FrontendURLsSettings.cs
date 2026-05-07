using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public class FrontendURLsSettings
    {
        [Required]
        public string BaseURL { get; set; } = null!;

        [Required]
        public string ConfirmEmailPath { get; set; } = null!;
    }
}

