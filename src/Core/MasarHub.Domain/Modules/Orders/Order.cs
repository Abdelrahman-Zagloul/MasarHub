using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            Status = OrderStatus.PendingPayment;
        }

        public static Order Create(Guid userId) => new Order(userId);
        public void SetOrderNumber(string orderNumber)
        {
            OrderNumber = Guard.AgainstNullOrWhiteSpace(orderNumber, nameof(orderNumber));
        }
        public void AddItem(OrderItem item)
        {
            EnsurePendingPayment();
            item = Guard.AgainstNull(item, nameof(item));

            if (_items.Any(x => x.CourseId == item.CourseId))
                throw new DomainException(ErrorCodes.Order.DuplicateCourse);

            _items.Add(item);
            RecalculateFinalAmount();
        }

        public void MarkPaid()
        {
            EnsurePendingPayment();

            if (!_items.Any())
                throw new DomainException(ErrorCodes.Order.EmptyOrder);

            Status = OrderStatus.Paid;
            MarkAsUpdated();
        }

        public void MarkFailed()
        {
            EnsurePendingPayment();
            Status = OrderStatus.Failed;
            MarkAsUpdated();
        }

        public void Cancel()
        {
            EnsurePendingPayment();
            Status = OrderStatus.Cancelled;
            MarkAsUpdated();
        }


        private void RecalculateFinalAmount()
        {
            FinalAmount = _items.Sum(x => x.FinalPrice);
        }
        private void EnsurePendingPayment()
        {
            if (Status != OrderStatus.PendingPayment)
                throw new DomainException(ErrorCodes.Order.InvalidStatusTransition);
        }
    }
}

