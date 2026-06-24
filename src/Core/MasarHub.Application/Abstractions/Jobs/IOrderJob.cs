using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IOrderJob : IScopedService
    {
        Task SendCreatedEmailAsync(Guid userId, string orderNumber, decimal finalAmount, Guid orderId);
        Task SendCancelledEmailAsync(Guid userId, string orderNumber, Guid orderId);
    }
}
