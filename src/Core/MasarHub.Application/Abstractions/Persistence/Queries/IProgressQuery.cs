using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Progress.Queries.GetCourseProgress;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IProgressQuery : IScopedService
    {
        Task<CreateProgressData> GetCreateProgressDataAsync(Guid userId, Guid courseId, Guid lessonId, CancellationToken ct = default);
        Task<int> GetModuleLessonCountAsync(Guid moduleId, CancellationToken ct = default);
        Task<int> GetCourseLessonCountAsync(Guid courseId, CancellationToken ct = default);
        Task<CourseProgressData> GetCourseProgressAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    }

    public sealed record CreateProgressData(bool HasEnrollment, bool LessonExists, bool AlreadyCompleted, Guid? ModuleId);
    public sealed record CourseProgressData(bool HasEnrollment, CourseProgress? CourseProgress, List<ModuleProgressItem> Modules);
}
