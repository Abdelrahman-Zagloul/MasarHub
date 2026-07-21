using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById
{
    public sealed record GetCourseReviewByIdQuery
    (
        Guid CourseId,
        Guid ReviewId
    ) : IRequest<Result<CourseReviewResponse>>;
}
