using MasarHub.Domain.Common.Events;
using MediatR;

namespace MasarHub.Application.Common.DomainEvents
{
    public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
        : INotification
        where TDomainEvent : IDomainEvent;
}
