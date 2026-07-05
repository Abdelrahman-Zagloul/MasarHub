using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IProgressJob : IScopedService
    {
        Task UpdateProgressAsync(Guid userId, Guid courseId, Guid moduleId, Guid lessonId);
    }
}
