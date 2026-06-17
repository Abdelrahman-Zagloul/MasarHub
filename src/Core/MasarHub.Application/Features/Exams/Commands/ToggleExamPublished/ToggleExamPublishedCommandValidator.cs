using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Exams.Commands.ToggleExamPublished
{
    public sealed class ToggleExamPublishedCommandValidator : AbstractValidator<ToggleExamPublishedCommand>
    {
        public ToggleExamPublishedCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");
        }
    }
}