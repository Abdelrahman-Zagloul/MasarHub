using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Orders
{
    public sealed class OrderItem
    {
        public Guid CourseId { get; private set; }
        public string CourseTitle { get; private set; } = null!;
        public decimal OriginalPrice { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal FinalPrice { get; private set; }
        public Guid? CouponId { get; private set; }

        private OrderItem() { }

        private OrderItem(Guid courseId, string courseTitle, decimal originalPrice, decimal discountAmount, Guid? couponId)
        {
            CourseId = courseId;
            CourseTitle = courseTitle;
            OriginalPrice = originalPrice;
            DiscountAmount = discountAmount;
            CouponId = couponId;
            FinalPrice = Math.Round(originalPrice - discountAmount, 2);
        }

        public static Result<OrderItem> Create(
            Guid courseId,
            string courseTitle,
            decimal originalPrice,
            decimal discountAmount,
            Guid? couponId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstNullOrWhiteSpace(courseTitle, nameof(courseTitle)),
                Guard.AgainstNegativeOrZero(originalPrice, nameof(originalPrice)),
                Guard.AgainstNegative(discountAmount, nameof(discountAmount))
            );

            if (error is not null)
                return error;

            if (couponId == null && discountAmount != 0)
                return OrderErrors.InvalidDiscount;

            if (couponId != null && discountAmount == 0)
                return OrderErrors.InvalidDiscount;

            if (discountAmount > originalPrice)
                return OrderErrors.InvalidDiscount;

            return new OrderItem(courseId, courseTitle, originalPrice, discountAmount, couponId);
        }
    }
}
