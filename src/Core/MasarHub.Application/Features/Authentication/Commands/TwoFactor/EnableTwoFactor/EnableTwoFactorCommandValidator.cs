using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    public sealed class EnableTwoFactorCommandValidator : AbstractValidator<EnableTwoFactorCommand>
    {
        public EnableTwoFactorCommandValidator()
        {
            RuleFor(x => x.Provider)
                .ValidEnum("Provider");
        }
    }
}
