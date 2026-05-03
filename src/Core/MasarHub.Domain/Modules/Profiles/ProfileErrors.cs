using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Profiles
{
    public static class ProfileErrors
    {
        public static readonly DomainError TooManySocialLinks = DomainError.TooManyItems("SocialLinks");
        public static readonly DomainError DuplicateSocialLink = DomainError.Duplicate("SocialLinks");
    }
}
