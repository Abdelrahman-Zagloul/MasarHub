namespace MasarHub.Domain.SharedKernel.Base
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; protected set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.CreateVersion7();
            CreatedAt = DateTimeOffset.UtcNow;
        }
        protected void MarkAsUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
    }
}
