using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Orders
{
    public static class OrderErrors
    {
        public const string NotFound = "order.not_found";
        public const string NotOwned = "order.not_owned";

        public static readonly DomainError InvalidDiscount = new("order.invalid_discount");
        public static readonly DomainError InvalidStatusTransition = new("order.invalid_status_transition");
        public static readonly DomainError DuplicateCourse = new("order.duplicate_course", "CourseId");
        public static readonly DomainError EmptyOrder = new("order.empty_order");
    }
}
