using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{

    public interface ICourseReviewQuery : IScopedService
    {
        Task<CreateReviewCheckResult> GetCreateReviewCheckAsync(Guid courseId, Guid userId, CancellationToken ct = default);
    }
    public sealed record CreateReviewCheckResult(bool IsEnrolled, bool HasExistingReview);
}
