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
                .MinLengthValidation(5, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .Required("Description")
                .MinLengthValidation(10, "Description")
                .MaxLengthValidation(2000, "Description");

            RuleFor(x => x.Price)
               .ValidPrice("Price", 0);

            RuleFor(x => x.CategoryId)
                .ValidGuid("CategoryId");

            RuleFor(x => x.Language)
                .ValidEnum("Language");

            RuleFor(x => x.Level)
                .ValidEnum("Level");
        }
    }
}
