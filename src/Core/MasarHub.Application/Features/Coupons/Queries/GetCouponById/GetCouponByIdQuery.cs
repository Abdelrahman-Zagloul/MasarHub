using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Queries.GetCouponById
{
    public sealed record GetCouponByIdQuery(Guid CourseId, Guid CouponId, Guid UserId) : IRequest<Result<CouponResponse>>;
}
