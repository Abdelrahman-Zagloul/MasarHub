using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IModuleQuery : IScopedService
    {
        Task<(bool CourseExists, bool IsOwner, int NextDisplayOrder)> GetCreationDataAsync(
            Guid courseId, Guid instructorId, CancellationToken cancellationToken);
    }
}
