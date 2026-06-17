using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Exams.Commands.UpdateExam
{
    public sealed record UpdateExamCommand
    (
        Guid ExamId,
        Guid InstructorId,
        string? Title,
        string? Description,
        int? MaxAttempts,
        int? PassingScorePercentage,
        int? DurationMinutes
    ) : IRequest<Result>;
}
