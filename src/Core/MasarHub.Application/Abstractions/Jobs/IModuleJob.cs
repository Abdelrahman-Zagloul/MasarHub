using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IModuleJob : IScopedService
    {
        Task CreateAnnouncementForNewModuleAsync(Guid courseId, string moduleTitle);
    }
}
