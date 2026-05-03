using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Profiles
{
    public sealed class InstructorProfile : BaseEntity
    {
        private readonly List<SocialLink> _socialLinks = [];

        public Guid UserId { get; private set; }
        public string Bio { get; private set; } = null!;
        public string? Headline { get; private set; }
        public IReadOnlyCollection<SocialLink> SocialLinks => _socialLinks.AsReadOnly();

        private InstructorProfile() { }

        private InstructorProfile(Guid userId, string bio)
        {
            UserId = userId;
            Bio = bio;
        }

        public static Result<InstructorProfile> Create(Guid userId, string bio)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstNullOrWhiteSpace(bio, nameof(bio))
            );

            if (error is not null)
                return error;

            return new InstructorProfile(userId, bio);
        }

        public Result UpdateBio(string bio)
        {
            var error = Guard.AgainstNullOrWhiteSpace(bio, nameof(bio));
            if (error is not null)
                return error;

            Bio = bio;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result UpdateHeadline(string? headline)
        {
            Headline = headline;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result AddSocialLink(SocialLink socialLink)
        {
            var error = Guard.AgainstNull(socialLink, nameof(socialLink));
            if (error is not null)
                return error;

            if (_socialLinks.Count >= 10)
                return ProfileErrors.TooManySocialLinks;

            if (_socialLinks.Any(x => x.Url == socialLink.Url))
                return ProfileErrors.DuplicateSocialLink;

            _socialLinks.Add(socialLink);
            MarkAsUpdated();
            return Result.Success();
        }

        public Result RemoveSocialLink(string url)
        {
            var link = _socialLinks.FirstOrDefault(x => x.Url == url);
            if (link is null)
                return Result.Success();

            _socialLinks.Remove(link);
            MarkAsUpdated();
            return Result.Success();
        }
    }
}
