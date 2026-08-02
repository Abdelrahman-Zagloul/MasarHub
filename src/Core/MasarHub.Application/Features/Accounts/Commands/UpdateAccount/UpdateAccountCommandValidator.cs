using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Accounts.Commands.UpdateAccount
{
    public sealed class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
    {
        public UpdateAccountCommandValidator()
        {
            RuleFor(x => x.UserId)
                .ValidGuid("UserId");

            RuleFor(x => x.PhoneNumber)
                .ValidEgyptianPhoneNumber("PhoneNumber");

            RuleFor(x => x.Gender)
                .ValidEnum("Gender");

            RuleFor(x => x.PreferredTwoFactorProvider)
                .ValidEnum("PreferredTwoFactorProvider");

            RuleFor(x => x)
                .Must(x => x.PhoneNumber is not null || x.Gender.HasValue || x.PreferredTwoFactorProvider.HasValue)
                .WithErrorCode("validation.at_least_one_field_required");
        }
    }
}
