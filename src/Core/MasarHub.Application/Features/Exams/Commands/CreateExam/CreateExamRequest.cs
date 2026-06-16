namespace MasarHub.Application.Features.Exams.Commands.CreateExam
{
    public sealed record CreateExamRequest
    (
        Guid? ModuleId,
        string Title,
        string? Description,
        int MaxAttempts,
        int PassingScorePercentage,
        int? DurationMinutes
    );
}
