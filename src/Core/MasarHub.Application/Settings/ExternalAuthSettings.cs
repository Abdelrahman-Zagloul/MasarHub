using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class ExternalAuthSettings
    {
        [Required]
        public GoogleSettings Google { get; init; } = new();

        [Required]
        public GitHubSettings GitHub { get; init; } = new();

        [Required]
        public LinkedInSettings LinkedIn { get; init; } = new();
    }

    public sealed class GoogleSettings
    {
        [Required]
        public string ClientId { get; init; } = default!;
    }

    public sealed class GitHubSettings
    {
        [Required]
        public string ClientId { get; init; } = default!;

        [Required]
        public string ClientSecret { get; init; } = default!;
        [Required]
        public string AuthorizeUrl { get; init; } = default!;
        [Required]
        public string TokenUrl { get; init; } = default!;

        [Required]
        public string UserUrl { get; init; } = default!;

        [Required]
        public string UserEmailsUrl { get; init; } = default!;
    }

    public sealed class LinkedInSettings
    {
        [Required]
        public string ClientId { get; init; } = default!;

        [Required]
        public string ClientSecret { get; init; } = default!;

        [Required]
        public string RedirectUrl { get; init; } = default!;

        [Required]
        public string TokenUrl { get; init; } = default!;

        [Required]
        public string UserInfoUrl { get; init; } = default!;
    }
}
