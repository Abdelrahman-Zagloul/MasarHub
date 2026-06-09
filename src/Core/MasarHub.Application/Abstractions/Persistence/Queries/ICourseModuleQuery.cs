using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseModuleQuery : IScopedService
    {
        Task<ModuleCreationData> GetCreationDataAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);

        Task<ModuleUpdateData> GetUpdateDataAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default);

        Task<bool> IsCourseOwnerAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
    }
    public sealed record ModuleCreationData(bool CourseExists, bool IsOwner, int NextDisplayOrder);
    public sealed record ModuleUpdateData(bool ModuleExists, bool IsOwner, Guid CourseId);
}
