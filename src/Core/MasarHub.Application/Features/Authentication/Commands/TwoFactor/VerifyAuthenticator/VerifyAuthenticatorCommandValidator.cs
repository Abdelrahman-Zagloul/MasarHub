using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator
{
    public sealed class VerifyAuthenticatorCommandValidator : AbstractValidator<VerifyAuthenticatorCommand>
    {
        public VerifyAuthenticatorCommandValidator()
        {
            RuleFor(x => x.Code)
                .Required("Code")
                .ValidOtpCode("Code");
        }
    }
}