using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Models.Notifications;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface ICreateNotificationJob : IScopedService
    {
        Task ExecuteAsync(CreateNotificationRequest request);
    }

}
