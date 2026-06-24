using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Orders.Events
{
    public sealed record OrderCancelledDomainEvent
    (
        Guid OrderId,
        Guid UserId,
        string OrderNumber
    ) : DomainEvent;
}
