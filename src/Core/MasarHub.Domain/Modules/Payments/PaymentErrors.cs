using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Payments
{
    public static class PaymentErrors
    {
        public static readonly DomainError InvalidStatusTransition = new("Payment.InvalidStatusTransition");
    }

    public static class CouponErrors
    {
        public static readonly DomainError Expired = new("Coupon.Expired");
        public static readonly DomainError Exhausted = new("Coupon.Exhausted");
        public static readonly DomainError NotApplicableToCourse = new("Coupon.NotApplicableToCourse", "CourseId");
        public static readonly DomainError InvalidPercentage = new("Coupon.InvalidPercentage", "Value");
        public static readonly DomainError InvalidExpiration = new("Coupon.InvalidExpiration", "ExpirationDate");
    }
}
