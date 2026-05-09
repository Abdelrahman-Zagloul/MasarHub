using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.ConfirmEmail
{
    internal sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .Required("Email")
                .ValidEmail("Email");

            RuleFor(x => x.Token)
                .Required("Token");
        }
    }
}
