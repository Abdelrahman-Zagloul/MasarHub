using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface ILessonJob : IScopedService
    {
        Task CleanUpLessonResourseAsync(Guid ModuleId, Guid LessonId);
    }

}
