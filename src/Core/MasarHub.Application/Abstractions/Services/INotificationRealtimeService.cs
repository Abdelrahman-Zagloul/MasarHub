namespace MasarHub.Application.Abstractions.Services
{
    public interface INotificationRealtimeService
    {
        Task SendToAdminsAsync(object data, CancellationToken cancellationToken = default);
    }
}
