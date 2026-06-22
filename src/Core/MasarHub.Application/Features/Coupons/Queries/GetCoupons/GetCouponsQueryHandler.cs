using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Queries.GetCoupons
{
    public sealed class GetCouponsQueryHandler : IRequestHandler<GetCouponsQuery, Result<List<CouponResponse>>>
    {
        private readonly ICouponQuery _couponQuery;

        public GetCouponsQueryHandler(ICouponQuery couponQuery)
        {
            _couponQuery = couponQuery;
        }

        public async Task<Result<List<CouponResponse>>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
        {
            var result = await _couponQuery.GetAllAsync(request, request.UserId, cancellationToken);

            if (!result.CourseExists)
                return Error.NotFound("course.not_found");

            if (!result.IsOwner)
                return Error.Forbidden("course.access_denied");

            return result.Coupons;
        }
    }
}
