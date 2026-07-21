namespace MasarHub.Application.Features.Reviews.Commands.CreateCourseReview
{
    public sealed record CreateCourseReviewResponse
    (
        Guid Id,
        Guid CourseId,
        Guid UserId,
        double Rating,
        string? ReviewContent,
        DateTimeOffset CreatedAt
    );
}
