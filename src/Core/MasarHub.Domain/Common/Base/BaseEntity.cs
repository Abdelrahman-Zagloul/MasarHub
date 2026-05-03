using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Common.Base
{
    public abstract class BaseEntity : IEntity
    {
        private readonly List<IDomainEvent> _domainEvents = [];
        public Guid Id { get; protected set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
            Id = Guid.CreateVersion7();
            CreatedAt = DateTimeOffset.UtcNow;
        }
        protected void MarkAsUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
