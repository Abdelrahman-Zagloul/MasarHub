using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Payments
{
    public sealed class Payment : BaseEntity
    {
        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public PaymentProvider Provider { get; private set; }
        public string? ExternalId { get; private set; } = null!;

        private Payment() { }

        private Payment(Guid userId, decimal amount, PaymentProvider provider)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            Amount = Guard.AgainstNegativeOrZero(amount, nameof(amount));
            Provider = Guard.AgainstEnumOutOfRange<PaymentProvider>(provider, nameof(provider));
            Status = PaymentStatus.Pending;
        }

        public static Payment Create(Guid userId, decimal amount, PaymentProvider provider)
        {
            return new Payment(userId, amount, provider);
        }

        public void MarkSucceeded(string externalId)
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException(ErrorCodes.Payment.InvalidStatusTransition);
            Status = PaymentStatus.Succeeded;
            ExternalId = externalId;
            MarkAsUpdated();
        }

        public void MarkFailed()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException(ErrorCodes.Payment.InvalidStatusTransition);
            Status = PaymentStatus.Failed;
            MarkAsUpdated();
        }

        public void MarkCancelled()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException(ErrorCodes.Payment.InvalidStatusTransition);

            Status = PaymentStatus.Cancelled;
            MarkAsUpdated();
        }

    }
}
