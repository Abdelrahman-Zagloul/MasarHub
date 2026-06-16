namespace MasarHub.Application.Features.Exams.Commands.UpdateExam
{
    public sealed record UpdateExamRequest
    (
        string? Title,
        string? Description,
        int? MaxAttempts,
        int? PassingScorePercentage,
        int? DurationMinutes
    );
}
