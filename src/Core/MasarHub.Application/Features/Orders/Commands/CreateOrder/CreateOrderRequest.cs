namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderRequest(List<CourseCoupon>? CourseCoupons);
}
