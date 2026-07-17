using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IAnnouncementJob : IScopedService
    {
        Task NotifyAsync(Guid announcementId, Guid courseId, string title, string content, AnnouncementImportance importance);
        Task PublishAsync(Guid announcementId, Guid courseId);
    }
}
