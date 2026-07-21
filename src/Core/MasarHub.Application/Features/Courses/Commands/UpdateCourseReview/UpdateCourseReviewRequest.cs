namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseReview
{
    public sealed record UpdateCourseReviewRequest
    (
        double? Rating,
        string? ReviewContent
    );
}
