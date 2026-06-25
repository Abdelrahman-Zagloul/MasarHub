using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Orders.Commands.CreateOrder;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IOrderQuery : IScopedService
    {
        Task<CreateOrderData> GetCreateOrderDataAsync(Guid userId, List<Guid> courseIds, List<CourseCoupon>? courseCoupons, CancellationToken ct = default);
        Task<Order?> GetOrderAsync(Guid orderId, Guid userId, CancellationToken ct = default);
    }

    public sealed record CreateOrderData(List<CourseCartData> Courses, List<Coupon> Coupons, List<Guid> ExistingUsageIds);
}
