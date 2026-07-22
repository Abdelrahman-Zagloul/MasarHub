using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviews
{
    public sealed record GetCourseReviewsQuery
    (
        Guid CourseId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery, IRequest<Result<PaginatedResult<CourseReviewResponse>>>;
}
