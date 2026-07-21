using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{

    public interface ICourseReviewQuery : IScopedService
    {
        Task<CreateReviewCheckResult> GetCreateReviewCheckAsync(Guid courseId, Guid userId, CancellationToken ct = default);
        Task<CourseReviewResponse?> GetByIdAsync(Guid courseId, Guid reviewId, CancellationToken ct = default);
    }
    public sealed record CreateReviewCheckResult(bool IsEnrolled, bool HasExistingReview);
}
