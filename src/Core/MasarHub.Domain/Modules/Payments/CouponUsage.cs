using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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
            CouponId = Guard.AgainstEmptyGuid(couponId, nameof(couponId));
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            UsedAt = DateTimeOffset.UtcNow;
        }

        public static CouponUsage Create(Guid couponId, Guid userId)
        {
            return new CouponUsage(couponId, userId);
        }
    }
}
