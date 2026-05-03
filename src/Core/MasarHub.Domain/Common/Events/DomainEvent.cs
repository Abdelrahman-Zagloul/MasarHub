namespace MasarHub.Domain.Common.Events
{
    public abstract record DomainEvent : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredOn { get; }
        protected DomainEvent()
        {
            EventId = Guid.CreateVersion7();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
