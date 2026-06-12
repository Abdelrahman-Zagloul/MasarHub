using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseModuleQuery : IScopedService
    {
        Task<ModuleCreationData> GetCreationDataAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<ModuleUpdateData> GetUpdateDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<ModuleDeleteData> GetDeleteDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct);
        Task<bool> IsCourseOwnerAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<List<Guid>> GetModuleIdsByCourseIdAsync(Guid courseId, CancellationToken ct = default);
        Task<bool> BulkUpdateDisplayOrderAsync(Guid courseId, IReadOnlyCollection<Guid> orderedModuleIds, CancellationToken ct = default);
    }

}
public sealed record ModuleCreationData(bool CourseExists, bool IsOwner, int NextDisplayOrder);
public sealed record ModuleUpdateData(bool ModuleExists, bool IsOwner);
public sealed record ModuleDeleteData(bool ModuleExists, bool IsOwner, bool HasLessons);
