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
                throw new DomainException(ErrorCodes.General.AlreadyDeleted);

            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
        }
        protected void Restore()
        {
            if (!IsDeleted)
                throw new DomainException(ErrorCodes.General.NotDeleted);

            IsDeleted = false;
            DeletedAt = null;
            MarkAsUpdated();
        }
    }
}
