namespace MasarHub.Application.Features.Courses.Commands.CreateCourseReview
{
    public sealed record CreateCourseReviewRequest
    (
        double Rating,
        string? ReviewContent
    );
}
