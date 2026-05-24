using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Models.Notifications;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface ICreateNotificationJob : IScopedService
    {
        Task ExecuteAsync(CreateNotificationRequest request);
    }

}
