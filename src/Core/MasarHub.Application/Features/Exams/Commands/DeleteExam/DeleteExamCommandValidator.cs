using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Exams.Commands.DeleteExam
{
    public sealed class DeleteExamCommandValidator : AbstractValidator<DeleteExamCommand>
    {
        public DeleteExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");
        }
    }
}
