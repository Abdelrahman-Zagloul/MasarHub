using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword
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
