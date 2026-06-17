using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourse
{
    public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.Title)
                .Required("Title")
                .ValidMinLength(5, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .Required("Description")
                .ValidMinLength(10, "Description")
                .ValidMaxLength(2000, "Description");

            RuleFor(x => x.Price)
               .ValidGreaterThanZero("Price");

            RuleFor(x => x.CategoryId)
                .ValidGuid("CategoryId");

            RuleFor(x => x.Language)
                .ValidEnum("Language");

            RuleFor(x => x.Level)
                .ValidEnum("Level");
        }
    }
}
