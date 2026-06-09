using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseModuleQuery : IScopedService
    {
        Task<(bool CourseExists, bool IsOwner, int NextDisplayOrder)> GetCreationDataAsync(
            Guid courseId, Guid instructorId, CancellationToken cancellationToken);

        Task<(bool ModuleExists, bool IsOwner, Guid CourseId)> GetUpdateDataAsync(
           Guid moduleId, Guid instructorId, CancellationToken cancellationToken);

        Task<bool> IsCourseOwnerAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken);
    }
}
