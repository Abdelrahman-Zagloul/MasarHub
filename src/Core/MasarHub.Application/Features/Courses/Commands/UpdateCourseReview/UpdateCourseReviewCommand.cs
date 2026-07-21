using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourseReview
{
    public sealed record UpdateCourseReviewCommand
    (
        Guid CourseId,
        Guid ReviewId,
        Guid UserId,
        double? Rating,
        string? ReviewContent
    ) : IRequest<Result>;
}
