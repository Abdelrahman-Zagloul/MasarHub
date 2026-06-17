using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor
{
    public sealed class RegisterInstructorCommandValidator : AbstractValidator<RegisterInstructorCommand>
    {
        public RegisterInstructorCommandValidator()
        {
            RuleFor(x => x.FullName)
                .Required("FullName")
                .ValidMaxLength(100, "FullName");

            RuleFor(x => x.Email)
                .Required("Email")
                .ValidEmail("Email");

            RuleFor(x => x.Password)
                .ValidPassword("Password");

            RuleFor(x => x.Gender)
                .ValidEnum("Gender");

            RuleFor(x => x.PhoneNumber)
                .Required("PhoneNumber")
                .ValidLength(11, "PhoneNumber");

            RuleFor(x => x.Headline)
                .Required("Headline")
                .ValidMaxLength(100, "Headline");

            RuleFor(x => x.Bio)
                .ValidMaxLength(2000, "Bio");

            RuleFor(x => x.Company)
                .ValidMaxLength(200, "Company");


            RuleFor(x => x.SocialLinks)
                .Must(x => x.Count <= 10)
                .WithErrorCode("validation.social_links_limit")
                .WithName("SocialLinks");

            RuleForEach(x => x.SocialLinks)
                .ChildRules(link =>
                {
                    link.RuleFor(x => x.PlatformName)
                        .Required("PlatformName")
                        .ValidMaxLength(50, "PlatformName");

                    link.RuleFor(x => x.Url)
                        .ValidUrl("Url");
                });
        }
    }
}

