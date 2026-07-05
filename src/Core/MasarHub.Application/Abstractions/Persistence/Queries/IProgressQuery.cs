using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IProgressQuery : IScopedService
    {
        Task<CreateProgressData> GetCreateProgressDataAsync(Guid userId, Guid courseId, Guid lessonId, CancellationToken ct = default);
        Task<int> GetModuleLessonCountAsync(Guid moduleId, CancellationToken ct = default);
        Task<int> GetCourseLessonCountAsync(Guid courseId, CancellationToken ct = default);
    }

    public sealed record CreateProgressData(bool HasEnrollment, bool LessonExists, bool AlreadyCompleted, Guid? ModuleId);
}
