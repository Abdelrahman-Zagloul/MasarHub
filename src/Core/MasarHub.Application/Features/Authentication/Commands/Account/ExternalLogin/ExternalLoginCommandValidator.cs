using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin
{
    public sealed class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
    {
        public ExternalLoginCommandValidator()
        {
            RuleFor(x => x.Provider)
                .ValidEnum("ExternalProvider");

            RuleFor(x => x.Token)
                .Required("Token");
        }
    }
}
