using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ILessonQuery : IScopedService
    {
        Task<LessonCreationData> GetCreationDataAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<LessonAttachmentCreationData> GetLessonAttachmentCreationAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default);
        Task<CourseState> GetCourseStateAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<LessonReorderData> GetReorderDataAsync(Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<bool> IsLessonOwnedByInstructorAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default);
        Task<List<Guid>> GetLessonIdsByModuleIdAsync(Guid moduleId, CancellationToken ct = default);
        Task<bool> BulkUpdateDisplayOrderAsync(Guid moduleId, IReadOnlyCollection<Guid> orderedLessonIds, CancellationToken ct = default);
    }
    public sealed record LessonCreationData(bool ModuleExist, bool IsOwner, int NextDisplayOrder);
    public sealed record LessonAttachmentCreationData(bool LessonExist, bool IsOwner, int AttachmentCount);
    public sealed record LessonReorderData(bool ModuleExist, bool IsOwner);
    public sealed record CourseState(bool ModuleExist, bool IsOwner, CourseStatus CourseStatus);

}
