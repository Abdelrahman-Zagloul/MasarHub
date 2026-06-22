using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.UpdateCoupon
{
    public sealed class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Result>
    {
        private readonly IRepository<Coupon> _couponRepository;
        private readonly ICouponQuery _couponQuery;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCouponCommandHandler(IRepository<Coupon> couponRepository, ICouponQuery couponQuery, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _couponQuery = couponQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            var couponData = await _couponQuery.GetCouponDataAsync(request.CouponId, request.UserId, cancellationToken);

            if (couponData == null)
                return Error.NotFound("coupon.not_found");

            if (!couponData.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (request.CourseId != couponData.CourseId)
                return Error.Conflict("coupon.course_mismatch");

            var coupon = await _couponRepository.GetByIdAsync(request.CouponId, cancellationToken);
            if (coupon == null)
                return Error.NotFound("coupon.not_found");

            if (request.Value.HasValue)
            {
                var valueResult = coupon.UpdateValue(request.Value.Value);
                if (valueResult.IsFailure)
                    return valueResult.Error;
            }

            if (request.ExpirationDate.HasValue)
            {
                var expirationResult = coupon.UpdateExpirationDate(request.ExpirationDate.Value);
                if (expirationResult.IsFailure)
                    return expirationResult.Error;
            }

            if (request.UsageLimit.HasValue)
            {
                var limitResult = coupon.UpdateUsageLimit(request.UsageLimit.Value);
                if (limitResult.IsFailure)
                    return limitResult.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
