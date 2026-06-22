using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.UpdateCoupon
{
    public sealed record UpdateCouponCommand
    (
        Guid CourseId,
        Guid CouponId,
        Guid UserId,
        decimal? Value,
        DateTimeOffset? ExpirationDate,
        int? UsageLimit
    ) : IRequest<Result>;
}
