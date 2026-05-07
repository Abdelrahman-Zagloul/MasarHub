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
                .ValidPassword("Password");

            RuleFor(x => x.Gender)
                .ValidEnum("Gender");

            RuleFor(x => x.PhoneNumber)
                .Required("PhoneNumber")
                .LengthValidation(11, "PhoneNumber");
        }
    }
}