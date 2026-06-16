using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Exams.Commands.UpdateExam
{
    public sealed class UpdateExamCommandValidator : AbstractValidator<UpdateExamCommand>
    {
        public UpdateExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .ValidGuid("ExamId");

            RuleFor(x => x.Title)
                .ValidMinLength(5, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMinLength(5, "Title")
                .ValidMaxLength(2000, "Description");

            RuleFor(x => x.MaxAttempts)
                .ValidRange(1, 100, "MaxAttempts");

            RuleFor(x => x.PassingScorePercentage)
                .ValidRange(1, 100, "PassingScorePercentage");

            RuleFor(x => x.DurationMinutes)
                .ValidGreaterThanZero("DurationMinutes");
        }
    }
}
