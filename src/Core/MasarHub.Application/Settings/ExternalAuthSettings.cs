using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class ExternalAuthSettings
    {
        [Required]
        public GoogleSettings Google { get; init; } = new();
    }

    public sealed class GoogleSettings
    {
        [Required]
        public string ClientId { get; init; } = default!;
    }
}
