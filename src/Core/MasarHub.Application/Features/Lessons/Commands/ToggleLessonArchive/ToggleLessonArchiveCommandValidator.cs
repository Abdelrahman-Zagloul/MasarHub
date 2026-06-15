using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.ToggleLessonArchive
{
    public sealed class ToggleLessonArchiveCommandValidator : AbstractValidator<ToggleLessonArchiveCommand>
    {
        public ToggleLessonArchiveCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");
        }
    }
}
