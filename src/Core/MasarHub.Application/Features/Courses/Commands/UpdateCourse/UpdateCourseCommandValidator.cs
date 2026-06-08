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
                .NotEmpty().WithErrorCode("validation.required").WithName("Title")
                .MaxLengthValidation(100, "Title")
                .When(x => x.Title != null);

            RuleFor(x => x.Description)
                .NotEmpty().WithErrorCode("validation.required").WithName("Description")
                .MaxLengthValidation(2000, "Description")
                .When(x => x.Description != null);

            RuleFor(x => x.Language)
                .ValidEnum("Language")
                .When(x => x.Language.HasValue);

            RuleFor(x => x.Level)
                .ValidEnum("Level")
                .When(x => x.Level.HasValue);

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
