using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Common.ValueObjects;

namespace MasarHub.Domain.Modules.Profiles
{
    public sealed record SocialLink : ValueObject
    {
        public string Platform { get; init; } = null!;
        public string Url { get; init; } = null!;

        private SocialLink() { }

        private SocialLink(string platform, string url)
        {
            Platform = platform;
            Url = url;
        }

        public static Result<SocialLink> Create(string platform, string url)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(platform, nameof(platform)),
                Guard.AgainstInvalidUrl(url, nameof(url))
            );

            if (error is not null)
                return error;

            return new SocialLink(platform, url);
        }
    }
}
