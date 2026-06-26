using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Abstractions.Services
{
    public interface IPaymentService : IScopedService
    {
        PaymentProvider Provider { get; }
        Task<Result<PaymentCreationResult>> CreateSessionAsync(Order order, IReadOnlyCollection<OrderItem> items, CancellationToken ct = default);
        Task<Result<PaymentWebhookValidationResult>> ValidateWebhookAsync(string rawBody, IDictionary<string, string> headers, CancellationToken ct = default);
    }
    public sealed record PaymentCreationResult(string ProviderReference, string PaymentUrl);
    public sealed record PaymentWebhookValidationResult(string ProviderReference, PaymentStatus Status);
}
