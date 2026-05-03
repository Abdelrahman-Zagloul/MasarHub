using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Orders
{
    public static class OrderErrors
    {
        public static readonly DomainError InvalidDiscount = new("Order.InvalidDiscount");
        public static readonly DomainError InvalidStatusTransition = new("Order.InvalidStatusTransition");
        public static readonly DomainError DuplicateCourse = new("Order.DuplicateCourse", "CourseId");
        public static readonly DomainError EmptyOrder = new("Order.EmptyOrder");
    }
}
