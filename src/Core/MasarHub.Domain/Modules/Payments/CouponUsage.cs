using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Payments
{
    public sealed class CouponUsage : BaseEntity
    {
        public Guid CouponId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTimeOffset UsedAt { get; private set; }

        private CouponUsage() { }

        private CouponUsage(Guid couponId, Guid userId)
        {
            CouponId = couponId;
            UserId = userId;
            UsedAt = DateTimeOffset.UtcNow;
        }

        public static Result<CouponUsage> Create(Guid couponId, Guid userId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(couponId, nameof(couponId)),
                Guard.AgainstEmptyGuid(userId, nameof(userId))
            );

            if (error is not null)
                return error;

            return new CouponUsage(couponId, userId);
        }
    }
}
