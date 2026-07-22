using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviews
{
    public sealed class GetCourseReviewsQueryValidator : PaginationValidator<GetCourseReviewsQuery>
    {
        public GetCourseReviewsQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");
        }
    }
}
