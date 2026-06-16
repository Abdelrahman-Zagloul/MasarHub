using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Exams.Commands.CreateExam
{
    public sealed class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");

            RuleFor(x => x.ModuleId)
                .ValidGuid("ModuleId");

            RuleFor(x => x.Title)
                .Required("Title")
                .ValidMinLength(5, "Title")
                .ValidMaxLength(200, "Title");

            RuleFor(x => x.Description)
                .ValidMinLength(5, "Description")
                .ValidMaxLength(2000, "Description");

            RuleFor(x => x.PassingScorePercentage)
                .ValidRange(0, 100, "PassingScorePercentage");

            RuleFor(x => x.MaxAttempts)
                .ValidRange(0, 100, "MaxAttempts");

            RuleFor(x => x.DurationMinutes)
                .ValidGreaterThanZero("DurationMinutes");
        }
    }
}
