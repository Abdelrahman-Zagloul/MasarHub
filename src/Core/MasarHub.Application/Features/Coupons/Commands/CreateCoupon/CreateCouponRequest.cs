using MasarHub.Domain.Modules.Payments;

namespace MasarHub.Application.Features.Coupons.Commands.CreateCoupon
{
    public sealed record CreateCouponRequest
    (
        string Code,
        decimal Value,
        DiscountType Type,
        DateTimeOffset ExpirationDate,
        int UsageLimit
    );
}
