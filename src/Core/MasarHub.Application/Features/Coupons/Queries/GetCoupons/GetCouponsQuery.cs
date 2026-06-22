using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Queries.GetCoupons
{
    public sealed record GetCouponsQuery
    (
        Guid CourseId,
        Guid UserId,
        CouponStatus? Status = null
    ) : IRequest<Result<List<CouponResponse>>>;
}
