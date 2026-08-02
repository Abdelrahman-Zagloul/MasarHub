using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
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

        public DomainResult UpdateFullName(string fullName)
        {
            var error = Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
            if (error != DomainError.None)
                return error;

            FullName = fullName;
            return DomainResult.Success();
        }

        public DomainResult UpdateProfileImage(string? publicId)
        {
            ProfileImagePublicId = publicId;
            return DomainResult.Success();
        }

        public DomainResult UpdatePhoneNumber(string phoneNumber)
        {
            var error = Guard.AgainstNullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            if (error != DomainError.None)
                return error;

            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = false;
            return DomainResult.Success();
        }

        public DomainResult UpdateGender(Gender gender)
        {
            var error = Guard.AgainstEnumOutOfRange(gender, nameof(gender));
            if (error != DomainError.None)
                return error;

            Gender = gender;
            return DomainResult.Success();
        }

        public DomainResult UpdatePreferredTwoFactorProvider(TwoFactorProvider provider)
        {
            if (!TwoFactorEnabled)
                return new DomainError("auth.2fa_not_enabled");

            var error = Guard.AgainstEnumOutOfRange(provider, nameof(provider));
            if (error != DomainError.None)
                return error;

            PreferredTwoFactorProvider = provider;
            return DomainResult.Success();
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
