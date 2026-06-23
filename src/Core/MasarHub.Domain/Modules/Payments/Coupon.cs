using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
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

        public static DomainResult<Coupon> Create(
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

        public DomainResult ApplyCoupon(Guid courseId)
        {
            var applicableResult = EnsureApplicableToCourse(courseId);
            if (applicableResult.IsFailure)
                return applicableResult.Error;

            var markResult = MarkUsed();
            if (markResult.IsFailure)
                return markResult.Error;

            return DomainResult.Success();
        }
        public DomainResult<decimal> CalculateDiscount(decimal price, Guid courseId)
        {
            var error = Guard.AgainstNegative(price, nameof(price));
            if (error != DomainError.None)
                return error;

            //var applicableResult = EnsureApplicableToCourse(courseId);
            //if (applicableResult.IsFailure)
            //    return applicableResult.Error;

            decimal discountAmount =
                Type == DiscountType.Percentage
                    ? price * Value / 100
                    : Value;

            return Math.Min(discountAmount, price);
        }
        public DomainResult UpdateValue(decimal newValue)
        {
            var error = Guard.AgainstNegativeOrZero(newValue, nameof(newValue));
            if (error != DomainError.None)
                return error;

            if (Type == DiscountType.Percentage && newValue > 100)
                return CouponErrors.InvalidPercentage;

            Value = newValue;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateExpirationDate(DateTimeOffset newExpirationDate)
        {
            if (newExpirationDate <= DateTimeOffset.UtcNow)
                return CouponErrors.InvalidExpiration;

            ExpirationDate = newExpirationDate;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult UpdateUsageLimit(int newUsageLimit)
        {
            var error = Guard.AgainstNegativeOrZero(newUsageLimit, nameof(newUsageLimit));
            if (error != DomainError.None)
                return error;

            if (newUsageLimit < UsedCount)
                return CouponErrors.UsageLimitBelowUsedCount;

            UsageLimit = newUsageLimit;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        private DomainResult EnsureApplicableToCourse(Guid courseId)
        {
            var error = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            if (error != DomainError.None)
                return error;

            if (IsExpired())
                return CouponErrors.Expired;

            if (IsExhausted())
                return CouponErrors.Exhausted;

            if (CourseId != courseId)
                return CouponErrors.NotApplicableToCourse;

            return DomainResult.Success();
        }
        private DomainResult MarkUsed()
        {
            if (IsExhausted())
                return CouponErrors.Exhausted;

            UsedCount++;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        private bool IsExpired() => DateTimeOffset.UtcNow > ExpirationDate;
        private bool IsExhausted() => UsedCount >= UsageLimit;
    }
}
