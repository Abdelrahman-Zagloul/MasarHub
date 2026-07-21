using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Commands.DeleteCourseReview
{
    public sealed record DeleteCourseReviewCommand
    (
        Guid CourseId,
        Guid ReviewId,
        Guid UserId
    ) : IRequest<Result>;
}
