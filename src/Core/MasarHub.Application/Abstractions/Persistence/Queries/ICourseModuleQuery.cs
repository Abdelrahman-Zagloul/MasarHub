using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Modules.Queries.GetModuleById;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseModuleQuery : IScopedService
    {
        Task<ModuleCreationData> GetCreationDataAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<ModuleUpdateData> GetUpdateDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<ModuleDeleteData> GetDeleteDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct);
        Task<List<Guid>> GetModuleIdsByCourseIdAsync(Guid courseId, CancellationToken ct = default);
        Task<bool> BulkUpdateDisplayOrderAsync(Guid courseId, IReadOnlyList<Guid> orderedModuleIds, CancellationToken ct = default);
        Task<ModuleDetailsResponse?> GetModuleByIdAsync(Guid courseId, Guid moduleId, CancellationToken ct = default);
    }

}
public sealed record ModuleCreationData(bool CourseExists, bool IsOwner, int NextDisplayOrder);
public sealed record ModuleUpdateData(bool ModuleExists, bool IsOwner);
public sealed record ModuleDeleteData(bool ModuleExists, bool IsOwner, bool HasLessons);
