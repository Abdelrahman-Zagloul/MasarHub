using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent
{
    public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
    {
        public RegisterStudentCommandValidator()
        {
            RuleFor(x => x.FullName)
                .Required("full_name")
                .MaxLengthValidation(100, "full_name");

            RuleFor(x => x.Email)
                .Required("email")
                .ValidEmail("email");

            RuleFor(x => x.Password)
                .Required("password")
                .MinLengthValidation(8, "password")
                .Matches("[0-9]")
                    .WithErrorCode("validation.password_requires_number")
                    .WithName("password")
                .Matches("[a-z]")
                    .WithErrorCode("validation.password_requires_lowercase")
                    .WithName("password");

            RuleFor(x => x.Gender)
                .ValidEnum("gender");

            RuleFor(x => x.PhoneNumber)
                .Required("phone_number")
                .LengthValidation(11, "phone_number");
        }
    }
}