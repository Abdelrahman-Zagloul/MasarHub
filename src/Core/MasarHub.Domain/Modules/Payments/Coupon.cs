using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Payments
{
    public sealed class Coupon : BaseEntity
    {
        public string Code { get; private set; } = null!;
        public decimal Value { get; private set; }
        public DiscountType Type { get; private set; }
        public Guid CourseId { get; private set; }
        public DateTimeOffset ExpirationDate { get; private set; }
        public int UsageLimit { get; private set; }
        public int UsedCount { get; private set; }

        private Coupon() { }

        private Coupon(
            string code,
            decimal value,
            DiscountType type,
            DateTimeOffset expirationDate,
            int usageLimit,
            Guid courseId)
        {
            Code = code;
            Value = value;
            Type = type;
            UsageLimit = usageLimit;
            CourseId = courseId;
            ExpirationDate = expirationDate;
        }

        public static Result<Coupon> Create(
            string code,
            decimal value,
            DiscountType type,
            DateTimeOffset expirationDate,
            int usageLimit,
            Guid courseId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(code, nameof(code)),
                Guard.AgainstNegativeOrZero(value, nameof(value)),
                Guard.AgainstEnumOutOfRange(type, nameof(type)),
                Guard.AgainstNegativeOrZero(usageLimit, nameof(usageLimit)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId))
            );

            if (error is not null)
                return error;

            if (expirationDate <= DateTimeOffset.UtcNow)
                return CouponErrors.InvalidExpiration;

            if (type == DiscountType.Percentage && value > 100)
                return CouponErrors.InvalidPercentage;

            return new Coupon(code, value, type, expirationDate, usageLimit, courseId);
        }

        public Result<decimal> ApplyCoupon(decimal price, Guid courseId)
        {
            var error = Guard.AgainstNegative(price, nameof(price));
            if (error is not null)
                return error;

            var applicableResult = EnsureApplicableToCourse(courseId);
            if (applicableResult.IsFailure)
                return applicableResult.Error;

            var discounted = Type == DiscountType.Percentage
                ? price - (price * Value / 100)
                : price - Value;

            return discounted < 0 ? 0 : discounted;
        }

        public Result EnsureApplicableToCourse(Guid courseId)
        {
            var error = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            if (error is not null)
                return error;

            if (IsExpired())
                return CouponErrors.Expired;

            if (IsExhausted())
                return CouponErrors.Exhausted;

            if (CourseId != courseId)
                return CouponErrors.NotApplicableToCourse;

            return Result.Success();
        }

        public Result MarkUsed()
        {
            if (IsExhausted())
                return CouponErrors.Exhausted;

            UsedCount++;
            MarkAsUpdated();
            return Result.Success();
        }

        private bool IsExpired() => DateTimeOffset.UtcNow > ExpirationDate;

        private bool IsExhausted() => UsedCount >= UsageLimit;
    }
}
