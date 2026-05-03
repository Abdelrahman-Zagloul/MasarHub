using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Categories.Events
{
    public sealed record CategoryCreatedEvent(Guid CategoryId) : DomainEvent;
}
