using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Services
{
    public sealed class MockPaymentService : IPaymentService
    {
        private readonly FrontendURLsSettings _frontendURLsSettings;

        public MockPaymentService(IOptions<FrontendURLsSettings> options)
        {
            _frontendURLsSettings = options.Value;
        }

        public Task<Result<PaymentCreationResult>> CreateSessionAsync(Guid orderId, decimal amount, Guid userId, CancellationToken ct = default)
        {
            var providerReference = $"mock_txn_{Guid.CreateVersion7():N}";
            var paymentUrl = $"{_frontendURLsSettings.BaseURL}/checkout/{orderId}?transactionId={providerReference}";

            return Task.FromResult(Result<PaymentCreationResult>.Success(new PaymentCreationResult(providerReference, paymentUrl)));
        }
    }
}
