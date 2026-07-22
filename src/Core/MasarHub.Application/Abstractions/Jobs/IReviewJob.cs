using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IReviewJob : IScopedService
    {
        Task NotifyInstructorOfNewReviewAsync(Guid courseId, Guid reviewId, double rating);
    }
}
