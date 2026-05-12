using MasarHub.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity
{
    internal static class IdentityErrorsMapper
    {
        public static List<Error> Map(IEnumerable<IdentityError> errors)
        {
            return errors.Select(MapIdentityToError).ToList();
        }

        private static Error MapIdentityToError(IdentityError error)
        {
            return error.Code switch
            {
                "DuplicateUserName" => Error.BadRequest("auth.user_name.already_exists", "userName"),
                "InvalidEmail" => Error.BadRequest("validation.invalid_email", "email"),
                "PasswordTooShort" => Error.BadRequest("validation.min_length", "password"),
                "PasswordRequiresDigit" => Error.BadRequest("validation.password_requires_number", "password"),
                "PasswordRequiresLower" => Error.BadRequest("validation.password_requires_lowercase", "password"),
                "PasswordRequiresUpper" => Error.BadRequest("validation.password_requires_uppercase", "password"),
                "PasswordRequiresNonAlphanumeric" => Error.BadRequest("validation.password_requires_special", "password"),
                "PasswordMismatch" => Error.BadRequest("auth.current_password_incorrect"),
                "InvalidToken" => Error.BadRequest("auth.invalid_or_expired_reset_token"),
                _ => Error.BadRequest("auth.unknown")
            };
        }
    }
}
