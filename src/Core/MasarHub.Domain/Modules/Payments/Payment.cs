using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Payments
{
    public sealed class Payment : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTimeOffset? PaidAt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public string? ExternalId { get; private set; }
        public string? IdempotencyKey { get; private set; }

        private Payment() { }

        private Payment(Guid orderId, decimal amount, PaymentProvider provider, string idempotencyKey)
        {
            OrderId = Guard.AgainstEmptyGuid(orderId, nameof(orderId));

            amount = Guard.AgainstNegativeOrZero(amount, nameof(amount));
            Amount = Math.Round(amount, 2);

            Provider = Guard.AgainstEnumOutOfRange(provider, nameof(provider));

            IdempotencyKey = Guard.AgainstNullOrWhiteSpace(idempotencyKey, nameof(idempotencyKey));

            Status = PaymentStatus.Pending;
        }

        public static Payment Create(Guid orderId, decimal amount, PaymentProvider provider, string idempotencyKey)
        {
            return new Payment(orderId, amount, provider, idempotencyKey);
        }

        public void MarkSucceeded(string externalId)
        {
            EnsurePending();

            ExternalId = Guard.AgainstNullOrWhiteSpace(externalId, nameof(externalId));
            Status = PaymentStatus.Succeeded;
            PaidAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
        }

        public void MarkFailed()
        {
            EnsurePending();

            Status = PaymentStatus.Failed;
            MarkAsUpdated();
        }

        public void MarkCancelled()
        {
            EnsurePending();

            Status = PaymentStatus.Cancelled;
            MarkAsUpdated();
        }

        private void EnsurePending()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException(ErrorCodes.Payment.InvalidStatusTransition);
        }
    }
}
