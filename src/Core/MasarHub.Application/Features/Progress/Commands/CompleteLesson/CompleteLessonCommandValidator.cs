using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Progress.Commands.CompleteLesson
{
    public sealed class CompleteLessonCommandValidator : AbstractValidator<CompleteLessonCommand>
    {
        public CompleteLessonCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.LessonId)
                .ValidGuid("LessonId");

            RuleFor(x => x.UserId)
                .ValidGuid("UserId");
        }
    }
}
