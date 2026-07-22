namespace MasarHub.Application.Features.Reviews.Commands.CreateCourseReview
{
    public sealed record CreateCourseReviewRequest
    (
        double Rating,
        string? ReviewContent
    );
}
