namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById
{
    public sealed record CourseReviewResponse
    (
        Guid Id,
        Guid CourseId,
        Guid UserId,
        string FullName,
        double Rating,
        string? ReviewContent,
        DateTimeOffset CreatedAt,
        DateTimeOffset? EditedAt
    );
}
