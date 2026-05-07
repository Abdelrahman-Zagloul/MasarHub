using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Profiles
{
    public sealed class InstructorProfile : BaseEntity
    {
        private readonly List<SocialLink> _socialLinks = [];

        public Guid UserId { get; private set; }
        public string Headline { get; private set; } = null!;
        public string? Bio { get; private set; }
        public string? Company { get; private set; }
        public VerificationStatus VerificationStatus { get; private set; }
        public IReadOnlyCollection<SocialLink> SocialLinks => _socialLinks.AsReadOnly();
        private InstructorProfile() { }
        private InstructorProfile(Guid userId, string headline, string? bio, string? company)
        {
            UserId = userId;
            Headline = headline;
            Bio = bio;
            Company = company;
            VerificationStatus = VerificationStatus.Pending;
        }

        public static Result<InstructorProfile> Create(Guid userId, string headline, string? bio, string? company)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstNullOrWhiteSpace(headline, nameof(headline))
            );

            if (error is not null)
                return error;

            return new InstructorProfile(userId, headline, bio, company);
        }

        public Result UpdateHeadline(string headline)
        {
            var error = Guard.AgainstNullOrWhiteSpace(headline, nameof(headline));
            if (error is not null)
                return error;

            Headline = headline;
            MarkAsUpdated();
            return Result.Success();
        }
        public Result UpdateBio(string? bio)
        {
            Bio = bio;
            MarkAsUpdated();
            return Result.Success();
        }
        public Result UpdateCompany(string? company)
        {
            Company = company;
            MarkAsUpdated();
            return Result.Success();
        }
        public Result Approve()
        {
            if (VerificationStatus == VerificationStatus.Approved)
                return ProfileErrors.AlreadyApproved;

            VerificationStatus = VerificationStatus.Approved;
            MarkAsUpdated();

            return Result.Success();
        }
        public Result Reject()
        {
            if (VerificationStatus == VerificationStatus.Rejected)
                return ProfileErrors.AlreadyRejected;

            VerificationStatus = VerificationStatus.Rejected;
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
