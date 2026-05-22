using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyRecoveryCode
{
    public sealed class VerifyRecoveryCodeCommandValidator : AbstractValidator<VerifyRecoveryCodeCommand>
    {
        public VerifyRecoveryCodeCommandValidator()
        {
            RuleFor(x => x.ChallengeId)
                .ValidGuid("ChallengeId");

            RuleFor(x => x.Code)
                .Required("Code");
        }
    }
}
