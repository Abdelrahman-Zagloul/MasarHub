using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.CreateCoupon
{
    public sealed record CreateCouponCommand
    (
        Guid CourseId,
        Guid UserId,
        string Code,
        decimal Value,
        DiscountType Type,
        DateTimeOffset ExpirationDate,
        int UsageLimit
    ) : IRequest<Result<CreateCouponResponse>>;
}
