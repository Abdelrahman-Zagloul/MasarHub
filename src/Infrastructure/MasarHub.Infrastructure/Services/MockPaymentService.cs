using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Settings;
using MasarHub.Domain.Modules.Payments;
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
        public PaymentProvider Provider => PaymentProvider.Mock;
        public Task<Result<PaymentCreationResult>> CreateSessionAsync(Guid orderId, decimal amount, Guid userId, CancellationToken ct = default)
        {
            var providerReference = $"mock_txn_{Guid.CreateVersion7():N}";
            var paymentUrl = $"{_frontendURLsSettings.BaseURL}/checkout/{orderId}?transactionId={providerReference}";

            return Task.FromResult(Result<PaymentCreationResult>.Success(new PaymentCreationResult(providerReference, paymentUrl)));
        }

        public async Task<Result<PaymentWebhookValidationResult>> ValidateWebhookAsync(string rawBody, IDictionary<string, string> headers, CancellationToken ct = default)
        {
            return new PaymentWebhookValidationResult("mock_txn_019eff88c2d6745b9cc0558dc200cda2", PaymentStatus.Succeeded);
            //return new PaymentWebhookValidationResult("mock_txn_019eff88c2d6745b9cc0558dc200cda2", PaymentStatus.Cancelled);
            //return new PaymentWebhookValidationResult("mock_txn_019eff88c2d6745b9cc0558dc200cda2", PaymentStatus.Expired);
            //return new PaymentWebhookValidationResult("mock_txn_019eff88c2d6745b9cc0558dc200cda2", PaymentStatus.Failed);
        }

        private sealed record MockWebhookPayload(string? TransactionId, string? Status);
    }
}
