using MasarHub.Application.Common.DI;
using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface ICreateNotificationJob : IScopedService
    {
        Task ExecuteAsync(Notification notification);
    }
}
