using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Exams.Commands.StartExamAttempt
{
    public sealed class StartExamAttemptCommandValidator : AbstractValidator<StartExamAttemptCommand>
    {
        public StartExamAttemptCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.UserId)
                .ValidGuid("UserId");
        }
    }
}
