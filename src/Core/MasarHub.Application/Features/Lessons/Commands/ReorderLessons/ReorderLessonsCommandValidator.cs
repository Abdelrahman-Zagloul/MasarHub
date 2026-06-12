using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Lessons.Commands.ReorderLessons
{
    public class ReorderLessonsCommandValidator : AbstractValidator<ReorderLessonsCommand>
    {
        public ReorderLessonsCommandValidator()
        {
            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.OrderedLessonIds)
                .Cascade(CascadeMode.Stop)
                .RequiredNonEmptyCollection("OrderedLessonIds")

                .Must(ids => ids.Count == ids.ToHashSet().Count)
                .WithErrorCode("lesson.duplicate_reorder_lesson_ids")

                .Must(ids => ids.All(id => id != Guid.Empty))
                .WithErrorCode("lesson.invalid_lesson_ids");
        }
    }
}
