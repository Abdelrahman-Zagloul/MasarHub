using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .ValidEmail("Email");

            RuleFor(x => x.Password)
                .Required("Password");
        }
    }
}
