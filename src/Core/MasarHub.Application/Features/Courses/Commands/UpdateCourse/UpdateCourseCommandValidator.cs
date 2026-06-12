using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Title)
                .MinLengthValidation(5, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MinLengthValidation(10, "Description")
                .MaxLengthValidation(2000, "Description");

            RuleFor(x => x.Language)
                .ValidEnum("Language");

            RuleFor(x => x.Level)
                .ValidEnum("Level");

            RuleFor(x => x.CategoryId)
                .ValidNullableGuid("CategoryId");

            RuleFor(x => x)
            .Must(HaveAtLeastOneUpdate)
            .WithErrorCode("validation.at_least_one_field_required");
        }
        private bool HaveAtLeastOneUpdate(UpdateCourseCommand command)
        {
            return command.Title != null ||
                   command.Description != null ||
                   command.Price.HasValue ||
                   command.Language.HasValue ||
                   command.Level.HasValue ||
                   command.CategoryId.HasValue;
        }
    }
}
