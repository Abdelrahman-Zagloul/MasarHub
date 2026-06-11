using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ILessonQuery : IScopedService
    {
        Task<LessonCreationData> GetCreationDataAsync(
            Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default);
        Task<LessonAttachmentCreationData> GetLessonAttachmentCreationAsync(
            Guid courseId, Guid moduleId, Guid lessonId, Guid instructorId, CancellationToken ct = default);
        Task<CourseState> GetCourseStateAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken ct = default);

    }
    public sealed record LessonCreationData(bool ModuleExist, bool IsOwner, int NextDisplayOrder);
    public sealed record LessonAttachmentCreationData(bool ModuleExist, bool LessonExist, bool IsOwner, int AttachmentCount);
    public sealed record CourseState(bool IsOwner, bool ModuleExist, CourseStatus CourseStatus);

}
