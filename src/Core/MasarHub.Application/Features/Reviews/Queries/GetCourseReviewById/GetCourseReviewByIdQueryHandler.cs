using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById
{
    public sealed class GetCourseReviewByIdQueryHandler : IRequestHandler<GetCourseReviewByIdQuery, Result<CourseReviewResponse>>
    {
        private readonly ICourseReviewQuery _courseReviewQuery;

        public GetCourseReviewByIdQueryHandler(ICourseReviewQuery courseReviewQuery)
        {
            _courseReviewQuery = courseReviewQuery;
        }

        public async Task<Result<CourseReviewResponse>> Handle(GetCourseReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _courseReviewQuery.GetByIdAsync(request.CourseId, request.ReviewId, cancellationToken);

            if (review == null)
                return Error.NotFound("course_review.not_found");

            return review;
        }
    }
}
