namespace MasarHub.Application.Features.Reviews.Commands.UpdateCourseReview
{
    public sealed record UpdateCourseReviewRequest
    (
        double? Rating,
        string? ReviewContent
    );
}
