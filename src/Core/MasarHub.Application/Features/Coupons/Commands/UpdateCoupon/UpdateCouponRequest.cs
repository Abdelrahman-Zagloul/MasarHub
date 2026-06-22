namespace MasarHub.Application.Features.Coupons.Commands.UpdateCoupon
{
    public sealed record UpdateCouponRequest
    (
        decimal? Value,
        DateTimeOffset? ExpirationDate,
        int? UsageLimit
    );
}
