using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.ToggleLessonPreview
{
    public sealed class ToggleLessonPreviewCommandValidator : AbstractValidator<ToggleLessonPreviewCommand>
    {
        public ToggleLessonPreviewCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");
        }
    }
}
