using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Commands.CreateCourseReview
{
    public sealed record CreateCourseReviewCommand
    (
        Guid CourseId,
        Guid UserId,
        double Rating,
        string? ReviewContent
    ) : IRequest<Result<CreateCourseReviewResponse>>;
}
