using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Orders.Events
{
    public sealed record OrderCreatedDomainEvent
    (
        Guid OrderId,
        Guid UserId,
        string OrderNumber,
        decimal FinalAmount
    ) : DomainEvent;
}
