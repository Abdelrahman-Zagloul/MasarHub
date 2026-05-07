using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Profiles
{
    public static class ProfileErrors
    {
        public static readonly DomainError TooManySocialLinks = DomainError.TooManyItems("SocialLinks");
        public static readonly DomainError DuplicateSocialLink = DomainError.Duplicate("SocialLinks");
        public static readonly DomainError AlreadyApproved = new DomainError("AlreadyApproved");
        public static readonly DomainError AlreadyRejected = new DomainError("AlreadyRejected");
    }
}
