using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendTwoFactorCode
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
