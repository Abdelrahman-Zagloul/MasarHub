namespace MasarHub.Application.Features.Exams.Commands.CreateExam
{
    public sealed record CreateExamResponse
    (
        Guid Id,
        string Title,
        string? Description,
        int PassingScorePercentage,
        int MaxAttempts,
        int? DurationMinutes,
        bool IsPublished,
        Guid CourseId,
        Guid? ModuleId
    );
}
