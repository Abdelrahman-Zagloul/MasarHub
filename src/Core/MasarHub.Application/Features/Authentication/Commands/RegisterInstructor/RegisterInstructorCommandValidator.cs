using FluentValidation;
using MasarHub.Application.Common.Validation;

namespace MasarHub.Application.Features.Authentication.Commands.RegisterInstructor
{
    public sealed class RegisterInstructorCommandValidator : AbstractValidator<RegisterInstructorCommand>
    {
        public RegisterInstructorCommandValidator()
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

            RuleFor(x => x.Headline)
                .Required("Headline")
                .MaxLengthValidation(100, "Headline");

            RuleFor(x => x.Bio)
                .MaxLengthValidation(2000, "Bio");

            RuleFor(x => x.Company)
                .MaxLengthValidation(200, "Compamny");


            RuleFor(x => x.SocialLinks)
                .Must(x => x.Count <= 10)
                .WithErrorCode("validation.social_links_limit")
                .WithName("SocialLinks");

            RuleForEach(x => x.SocialLinks)
                .ChildRules(link =>
                {
                    link.RuleFor(x => x.PlatformName)
                        .Required("PlatformName")
                        .MaxLengthValidation(50, "PlatformName");

                    link.RuleFor(x => x.Url)
                        .ValidUrl("Url");
                });
        }
    }
}

