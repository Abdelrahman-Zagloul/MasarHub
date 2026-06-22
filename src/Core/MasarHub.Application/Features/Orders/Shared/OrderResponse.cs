using MasarHub.Domain.Modules.Orders;

namespace MasarHub.Application.Features.Orders.Shared
{
    public sealed record OrderResponse
    (
        Guid Id,
        string OrderNumber,
        decimal FinalAmount,
        OrderStatus Status,
        DateTimeOffset CreatedAt,
        IReadOnlyList<OrderItemResponse> OrderItems
    );
}
