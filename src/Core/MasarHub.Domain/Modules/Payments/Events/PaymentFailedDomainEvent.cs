using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Payments.Events
{
    public sealed record PaymentFailedDomainEvent
    (
        Guid PaymentId,
        Guid OrderId,
        Guid UserId,
        string OrderNumber,
        decimal Amount,
        PaymentProvider Provider,
        string ProviderReference
    ) : DomainEvent;
}
