using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Orders.Shared;
using MediatR;

namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand
    (
        Guid UserId,
        List<CourseCoupon>? Coupons
    ) : IRequest<Result<OrderResponse>>;
    public sealed record CourseCoupon(string CouponCode, Guid CourseId);
}
