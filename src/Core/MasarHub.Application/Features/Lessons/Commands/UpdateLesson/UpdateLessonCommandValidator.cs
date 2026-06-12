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
                .MinLengthValidation(10, "Title")
                .MaxLengthValidation(200, "Title");

            RuleFor(x => x.Description)
                .MaxLengthValidation(2000, "Description");

            RuleFor(x => x)
                .Must(x => x.Title != null || x.Description != null)
                .WithErrorCode("validation.at_least_one_field_required");
        }
    }
}
