using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            Code = Guard.AgainstNullOrWhiteSpace(code, nameof(code));
            Value = Guard.AgainstNegativeOrZero(value, nameof(value));
            Type = Guard.AgainstEnumOutOfRange(type, nameof(type));
            UsageLimit = Guard.AgainstNegativeOrZero(usageLimit, nameof(usageLimit));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));

            if (expirationDate <= DateTimeOffset.UtcNow)
                throw new DomainException(ErrorCodes.Coupon.InvalidExpiration);
            ExpirationDate = expirationDate;

            ValidateDiscount(value, type);
        }

        public static Coupon Create(
            string code,
            decimal value,
            DiscountType type,
            DateTimeOffset expirationDate,
            int usageLimit,
            Guid courseId)
        {
            return new Coupon(code, value, type, expirationDate, usageLimit, courseId);
        }

        public decimal ApplyCoupon(decimal price, Guid courseId)
        {
            EnsureApplicableToCourse(courseId);

            var discounted = Type == DiscountType.Percentage
                ? price - (price * Value / 100)
                : price - Value;

            return discounted < 0 ? 0 : discounted;
        }

        public void EnsureApplicableToCourse(Guid courseId)
        {
            if (IsExpired())
                throw new DomainException(ErrorCodes.Coupon.Expired);

            if (IsExhausted())
                throw new DomainException(ErrorCodes.Coupon.Exhausted);

            if (CourseId != courseId)
                throw new DomainException(ErrorCodes.Coupon.NotApplicableToCourse);
        }

        public void MarkUsed()
        {
            if (IsExhausted())
                throw new DomainException(ErrorCodes.Coupon.Exhausted);

            UsedCount++;
            MarkAsUpdated();
        }

        private void ValidateDiscount(decimal value, DiscountType type)
        {
            if (type == DiscountType.Percentage && value > 100)
                throw new DomainException(ErrorCodes.Coupon.InvalidPercentage);
        }

        private bool IsExpired() => DateTimeOffset.UtcNow > ExpirationDate;

        private bool IsExhausted() => UsedCount >= UsageLimit;
    }
}