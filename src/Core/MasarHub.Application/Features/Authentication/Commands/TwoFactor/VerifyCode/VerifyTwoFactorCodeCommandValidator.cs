using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyCode
{
    public sealed class VerifyTwoFactorCodeCommandValidator : AbstractValidator<VerifyTwoFactorCodeCommand>
    {
        public VerifyTwoFactorCodeCommandValidator()
        {
            RuleFor(x => x.ChallengeId)
                .ValidGuid("ChallengeId");

            RuleFor(x => x.Code)
                .Required("Code")
                .ValidOtpCode("Code");
        }
    }
}
