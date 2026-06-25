using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Payments
{
    public sealed class Payment : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTimeOffset? PaidAt { get; private set; }
        public PaymentStatus Status { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public string? ProviderReference { get; private set; }

        private Payment() { }

        private Payment(Guid orderId, decimal amount, PaymentProvider provider, string? providerReference)
        {
            OrderId = orderId;
            Amount = Math.Round(amount, 2);
            Provider = provider;
            ProviderReference = providerReference;
            Status = PaymentStatus.Pending;
        }
        public static DomainResult<Payment> Create(Guid orderId, decimal amount, PaymentProvider provider, string? providerReference)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(orderId, nameof(orderId)),
                Guard.AgainstNegativeOrZero(amount, nameof(amount)),
                Guard.AgainstEnumOutOfRange(provider, nameof(provider)),
                Guard.AgainstNullOrWhiteSpace(providerReference, nameof(providerReference))
            );

            if (error is not null)
                return error;

            return new Payment(orderId, amount, provider, providerReference);
        }
        public DomainResult UpdateProviderReference(string providerReference)
        {
            var error = Guard.AgainstNullOrWhiteSpace(providerReference, nameof(providerReference));
            if (error != DomainError.None)
                return error;

            ProviderReference = providerReference;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkSucceeded()
        {
            var pendingResult = EnsurePending();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = PaymentStatus.Succeeded;
            PaidAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();

            return DomainResult.Success();
        }
        public DomainResult MarkFailed()
        {
            var pendingResult = EnsurePending();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = PaymentStatus.Failed;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkExpired()
        {
            var pendingResult = EnsurePending();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = PaymentStatus.Expired;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkCancelled()
        {
            var pendingResult = EnsurePending();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = PaymentStatus.Cancelled;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        private DomainResult EnsurePending()
        {
            return Status == PaymentStatus.Pending
                ? DomainResult.Success()
                : PaymentErrors.InvalidStatusTransition;
        }
    }
}
