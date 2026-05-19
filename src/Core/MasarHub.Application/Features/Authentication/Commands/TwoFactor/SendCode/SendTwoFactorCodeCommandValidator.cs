using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendCode
{
    public sealed class SendTwoFactorCodeCommandValidator : AbstractValidator<SendTwoFactorCodeCommand>
    {
        public SendTwoFactorCodeCommandValidator()
        {
            RuleFor(x => x.ChallengeId.ToString())
                .Required("ChallengeId");
        }
    }
}
