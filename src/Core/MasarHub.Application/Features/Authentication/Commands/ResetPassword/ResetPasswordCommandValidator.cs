using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.ResetPassword
{

    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .ValidEmail("Email");

            RuleFor(x => x.Token)
                .Required("Token");

            RuleFor(x => x.NewPassword)
                .ValidPassword("NewPassword");
        }
    }
}
