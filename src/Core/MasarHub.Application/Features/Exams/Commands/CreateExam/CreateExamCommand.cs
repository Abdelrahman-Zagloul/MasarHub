using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.CreateExam
{
    public sealed record CreateExamCommand
    (
        Guid CourseId,
        Guid InstructorId,
        string Title,
        int PassingScorePercentage,
        int MaxAttempts,
        Guid? ModuleId,
        string? Description,
        int? DurationMinutes
    ) : IRequest<Result<CreateExamResponse>>;
}
