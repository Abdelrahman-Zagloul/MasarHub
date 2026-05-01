using MasarHub.Domain.SharedKernel.Events;
using MediatR;

namespace MasarHub.Application.Common.Events
{
    public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
        : INotification
        where TDomainEvent : IDomainEvent;
}
