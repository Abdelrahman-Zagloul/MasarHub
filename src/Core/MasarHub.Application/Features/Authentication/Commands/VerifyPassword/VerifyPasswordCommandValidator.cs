using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.VerifyPassword
{
    public sealed class VerifyPasswordCommandValidator : AbstractValidator<VerifyPasswordCommand>
    {
        public VerifyPasswordCommandValidator()
        {
            RuleFor(x => x.Password)
                .Required("Password");
        }
    }
}
