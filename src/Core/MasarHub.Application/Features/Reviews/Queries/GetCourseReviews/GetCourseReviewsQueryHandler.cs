using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviews
{
    public sealed class GetCourseReviewsQueryHandler : IRequestHandler<GetCourseReviewsQuery, Result<PaginatedResult<CourseReviewResponse>>>
    {
        private readonly ICourseReviewQuery _courseReviewQuery;

        public GetCourseReviewsQueryHandler(ICourseReviewQuery courseReviewQuery)
        {
            _courseReviewQuery = courseReviewQuery;
        }

        public async Task<Result<PaginatedResult<CourseReviewResponse>>> Handle(GetCourseReviewsQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _courseReviewQuery.GetAllAsync(request, cancellationToken);

            return PaginatedResult<CourseReviewResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
