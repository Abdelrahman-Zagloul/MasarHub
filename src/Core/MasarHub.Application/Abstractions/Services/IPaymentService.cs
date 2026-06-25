using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.Services
{
    public interface IPaymentService : IScopedService
    {
        Task<Result<PaymentCreationResult>> CreateSessionAsync(Guid orderId, decimal amount, Guid userId, CancellationToken ct = default);
    }
    public sealed record PaymentCreationResult(string ProviderReference, string PaymentUrl);
}
