namespace MasarHub.Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed record CourseProgressResponse
    (
        Guid CourseId,
        int CompletedLessons,
        int TotalLessons,
        bool IsCompleted,
        DateTimeOffset? CompletedAt,
        List<ModuleProgressItem> Modules
    );
    public sealed record ModuleProgressItem
    (
        Guid ModuleId,
        string ModuleTitle,
        int DisplayOrder,
        int CompletedLessons,
        int TotalLessons
    );
}
