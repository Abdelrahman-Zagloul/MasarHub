using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Coupons.Queries.GetCoupons;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Queries.GetCouponById
{
    public sealed class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, Result<CouponResponse>>
    {
        private readonly ICouponQuery _couponQuery;

        public GetCouponByIdQueryHandler(ICouponQuery couponQuery)
        {
            _couponQuery = couponQuery;
        }

        public async Task<Result<CouponResponse>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _couponQuery.GetByIdAsync(request.CouponId, request.CourseId, request.UserId, cancellationToken);

            if (result.Coupon is null)
                return Error.NotFound("coupon.not_found");

            if (!result.IsOwner)
                return Error.Forbidden("course.access_denied");

            return result.Coupon;
        }
    }
}
