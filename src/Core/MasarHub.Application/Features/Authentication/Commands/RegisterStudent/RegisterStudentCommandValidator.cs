using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterStudent
{
    public sealed class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
    {
        public RegisterStudentCommandValidator()
        {
            RuleFor(x => x.FullName)
                .Required("FullName")
                .MaxLengthValidation(100, "FullName");

            RuleFor(x => x.Email)
                .Required("Email")
                .ValidEmail("Email");

            RuleFor(x => x.Password)
                .Required("password")
                .MinLengthValidation(8, "Password")
                .Matches("[0-9]")
                    .WithErrorCode("validation.password_requires_number")
                    .OverridePropertyName("Password")
                .Matches("[a-z]")
                    .WithErrorCode("validation.password_requires_lowercase")
                    .OverridePropertyName("Password");

            RuleFor(x => x.Gender)
                .ValidEnum("Gender");

            RuleFor(x => x.PhoneNumber)
                .Required("PhoneNumber")
                .LengthValidation(11, "PhoneNumber");
        }
    }
}