using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed class UpdateLessonCommandValidator : AbstractValidator<UpdateLessonCommand>
    {
        public UpdateLessonCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");

            RuleFor(x => x.Title)
                .ValidMinLength(10, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMaxLength(2000, "Description");

            RuleFor(x => x)
                .Must(x => x.Title != null || x.Description != null)
                .WithErrorCode("validation.at_least_one_field_required");
        }
    }
}
