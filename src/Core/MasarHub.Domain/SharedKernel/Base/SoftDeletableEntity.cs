using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.SharedKernel.Base
{
    public abstract class SoftDeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }

        protected void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new DomainException("Entity already deleted.");

            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
        }
        protected void Restore()
        {
            if (!IsDeleted)
                throw new DomainException("Entity is not deleted.");

            IsDeleted = false;
            DeletedAt = null;
            MarkAsUpdated();
        }
    }
}
