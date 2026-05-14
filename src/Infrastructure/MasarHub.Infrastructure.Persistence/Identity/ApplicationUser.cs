using MasarHub.Domain.Modules.Profiles;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Persistence.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public string? ProfileImagePublicId { get; private set; }
        public Gender Gender { get; set; }
        public TwoFactorProvider? PreferredTwoFactorProvider { get; private set; }
        public void UpdateFullName(string fullName)
        {
            FullName = fullName;
        }
        public void UpdateProfileImage(string? publicId)
        {
            ProfileImagePublicId = publicId;
        }
        public void EnableTwoFactor(TwoFactorProvider provider)
        {
            TwoFactorEnabled = true;
            PreferredTwoFactorProvider = provider;
        }
        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            PreferredTwoFactorProvider = null;
        }
    }
}
