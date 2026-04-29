using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            CourseTitle = Guard.AgainstNullOrWhiteSpace(courseTitle, nameof(courseTitle));
            OriginalPrice = Guard.AgainstNegativeOrZero(originalPrice, nameof(originalPrice));
            discountAmount = Guard.AgainstNegative(discountAmount, nameof(discountAmount));

            if (couponId == null && discountAmount != 0)
                throw new DomainException(ErrorCodes.Order.InvalidDiscount);

            if (couponId != null && discountAmount == 0)
                throw new DomainException(ErrorCodes.Order.InvalidDiscount);

            if (discountAmount > originalPrice)
                throw new DomainException(ErrorCodes.Order.InvalidDiscount);

            DiscountAmount = discountAmount;
            CouponId = couponId;
            FinalPrice = Math.Round(originalPrice - discountAmount, 2);
        }

        public static OrderItem Create(Guid courseId, string courseTitle, decimal originalPrice, decimal discountAmount, Guid? couponId)
            => new OrderItem(courseId, courseTitle, originalPrice, discountAmount, couponId);
    }
}