using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.ValueObjects;

namespace MasarHub.Domain.Modules.Profiles
{
    public sealed record SocialLink : ValueObject
    {
        public string Platform { get; init; } = null!;
        public string Url { get; init; } = null!;

        private SocialLink() { }
        private SocialLink(string platform, string url)
        {
            Platform = Guard.AgainstNullOrWhiteSpace(platform, nameof(platform));
            Url = Guard.AgainstInvalidUrl(url, nameof(url));
        }

        public static SocialLink Create(string platform, string url)
        {
            return new SocialLink(platform, url);
        }
    }
}