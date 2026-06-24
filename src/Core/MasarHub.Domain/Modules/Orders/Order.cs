using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Orders.Events;

namespace MasarHub.Domain.Modules.Orders
{
    public sealed class Order : SoftDeletableEntity
    {
        private readonly List<OrderItem> _items = new();

        public Guid UserId { get; private set; }
        public string OrderNumber { get; private set; } = null!;
        public decimal FinalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public bool IsFree => FinalAmount == 0;
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }

        private Order(Guid userId, string orderNumber)
        {
            UserId = userId;
            OrderNumber = orderNumber;
            Status = OrderStatus.PendingPayment;
        }

        public static DomainResult<Order> Create(Guid userId, string orderNumber)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstNullOrWhiteSpace(orderNumber, nameof(orderNumber))
            );
            if (error != null)
                return error;

            var order = new Order(userId, orderNumber);
            order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, userId, orderNumber, order.FinalAmount));
            return order;
        }
        public DomainResult AddItem(OrderItem item)
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            var error = Guard.AgainstNull(item, nameof(item));
            if (error != DomainError.None)
                return error;

            if (_items.Any(x => x.CourseId == item.CourseId))
                return OrderErrors.DuplicateCourse;

            _items.Add(item);
            RecalculateFinalAmount();
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkPaid()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            if (!_items.Any())
                return OrderErrors.EmptyOrder;

            Status = OrderStatus.Paid;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult MarkFailed()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = OrderStatus.Failed;
            MarkAsUpdated();
            return DomainResult.Success();
        }
        public DomainResult Cancel()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = OrderStatus.Cancelled;
            MarkAsUpdated();
            RaiseDomainEvent(new OrderCancelledDomainEvent(Id, UserId, OrderNumber));
            return DomainResult.Success();
        }
        public DomainResult Delete()
        {
            if (Status != OrderStatus.PendingPayment && Status != OrderStatus.Cancelled)
                return OrderErrors.InvalidStatusForDeletion;

            MarkAsDeleted();
            return DomainResult.Success();
        }

        private void RecalculateFinalAmount()
        {
            FinalAmount = _items.Sum(x => x.FinalPrice);
        }
        private DomainResult EnsurePendingPayment()
        {
            return Status == OrderStatus.PendingPayment
                ? DomainResult.Success()
                : OrderErrors.InvalidStatusTransition;
        }
    }
}
