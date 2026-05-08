using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Profiles
{
    public static class ProfileErrors
    {
        public static readonly DomainError TooManySocialLinks = new("profile.too_many_social_links");
        public static readonly DomainError DuplicateSocialLink = new("profile.duplicate_social_link");
        public static readonly DomainError AlreadyApproved = new("profile.already_approved");
        public static readonly DomainError AlreadyRejected = new("profile.already_rejected");
    }
}
