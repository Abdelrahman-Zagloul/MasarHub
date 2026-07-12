using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IAnnouncementJob : IScopedService
    {
        Task ExecuteAsync(Guid announcementId, Guid courseId, string title, string content, AnnouncementImportance importance);
    }
}
