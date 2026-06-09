using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ILessonQuery : IScopedService
    {
        Task<LessonCreationData> GetCreationDataAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default);
    }
    public sealed record LessonCreationData(bool ModuleExist, bool IsOwner, int NextDisplayOrder);

}
