using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IModuleJobService : IScopedService
    {
        Task CreateAnnouncementForNewModuleAsync(Guid courseId, string moduleTitle);
    }
}
