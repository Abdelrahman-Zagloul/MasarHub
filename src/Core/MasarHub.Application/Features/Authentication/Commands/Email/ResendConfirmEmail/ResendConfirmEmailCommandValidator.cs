using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail
{
    public sealed class ResendConfirmEmailCommandValidator : AbstractValidator<ResendConfirmEmailCommand>
    {
        public ResendConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .Required("Email")
                .ValidEmail("Email");
        }
    }
}
