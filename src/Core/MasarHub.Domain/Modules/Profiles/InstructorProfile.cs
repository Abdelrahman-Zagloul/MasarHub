using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            Bio = Guard.AgainstNullOrWhiteSpace(bio, nameof(bio));
        }

        public static InstructorProfile Create(Guid userId, string bio)
        {
            return new InstructorProfile(userId, bio);
        }

        public void UpdateBio(string bio)
        {
            Bio = Guard.AgainstNullOrWhiteSpace(bio, nameof(bio));
            MarkAsUpdated();
        }

        public void UpdateHeadline(string? headline)
        {
            Headline = headline;
            MarkAsUpdated();
        }
        public void AddSocialLink(SocialLink socialLink)
        {
            if (_socialLinks.Count >= 10)
                throw new DomainException(ErrorCodes.General.TooManyItems);

            if (_socialLinks.Any(x => x.Url == socialLink.Url))
                throw new DomainException(ErrorCodes.General.Duplicate);

            _socialLinks.Add(socialLink);
            MarkAsUpdated();
        }
        public void RemoveSocialLink(string url)
        {
            var link = _socialLinks.FirstOrDefault(x => x.Url == url);
            if (link is null) return;

            _socialLinks.Remove(link);
            MarkAsUpdated();
        }
    }
}