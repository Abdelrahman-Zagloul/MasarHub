using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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

        private Order(Guid userId)
        {
            UserId = userId;
            Status = OrderStatus.PendingPayment;
        }

        public static Result<Order> Create(Guid userId)
        {
            var error = Guard.AgainstEmptyGuid(userId, nameof(userId));
            if (error is not null)
                return error;

            return new Order(userId);
        }

        public Result SetOrderNumber(string orderNumber)
        {
            var error = Guard.AgainstNullOrWhiteSpace(orderNumber, nameof(orderNumber));
            if (error is not null)
                return error;

            OrderNumber = orderNumber;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result AddItem(OrderItem item)
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            var error = Guard.AgainstNull(item, nameof(item));
            if (error is not null)
                return error;

            if (_items.Any(x => x.CourseId == item.CourseId))
                return OrderErrors.DuplicateCourse;

            _items.Add(item);
            RecalculateFinalAmount();
            MarkAsUpdated();
            return Result.Success();
        }

        public Result MarkPaid()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            if (!_items.Any())
                return OrderErrors.EmptyOrder;

            Status = OrderStatus.Paid;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result MarkFailed()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = OrderStatus.Failed;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Cancel()
        {
            var pendingResult = EnsurePendingPayment();
            if (pendingResult.IsFailure)
                return pendingResult;

            Status = OrderStatus.Cancelled;
            MarkAsUpdated();
            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();

        private void RecalculateFinalAmount()
        {
            FinalAmount = _items.Sum(x => x.FinalPrice);
        }

        private Result EnsurePendingPayment()
        {
            return Status == OrderStatus.PendingPayment
                ? Result.Success()
                : OrderErrors.InvalidStatusTransition;
        }
    }
}
