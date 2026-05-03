using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Common.Base
{
    public abstract class SoftDeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }

        protected Result MarkAsDeleted()
        {
            if (IsDeleted)
                return DomainError.AlreadyDeleted();

            IsDeleted = true;
            DeletedAt = DateTimeOffset.UtcNow;

            MarkAsUpdated();
            return Result.Success();
        }

        protected Result Restore()
        {
            if (!IsDeleted)
                return DomainError.NotDeleted();

            IsDeleted = false;
            DeletedAt = null;
            MarkAsUpdated();

            return Result.Success();
        }
    }
}
