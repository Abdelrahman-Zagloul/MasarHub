using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail
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
