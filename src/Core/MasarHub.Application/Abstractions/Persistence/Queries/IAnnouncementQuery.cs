using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IAnnouncementQuery : IScopedService
    {
        Task<List<Guid>> GetEnrolledUserIdsAsync(Guid courseId, CancellationToken ct = default);
    }
}
