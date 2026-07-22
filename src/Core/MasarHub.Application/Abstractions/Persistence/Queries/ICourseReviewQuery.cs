using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviews;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{

    public interface ICourseReviewQuery : IScopedService
    {
        Task<CreateReviewCheckResult> GetCreateReviewCheckAsync(Guid courseId, Guid userId, CancellationToken ct = default);
        Task<CourseReviewResponse?> GetByIdAsync(Guid courseId, Guid reviewId, CancellationToken ct = default);
        Task<PagedResult<CourseReviewResponse>> GetAllAsync(GetCourseReviewsQuery query, CancellationToken ct = default);
    }
    public sealed record CreateReviewCheckResult(bool IsEnrolled, bool HasExistingReview);
}
