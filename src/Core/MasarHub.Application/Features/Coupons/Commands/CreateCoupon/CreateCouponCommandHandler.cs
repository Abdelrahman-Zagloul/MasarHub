using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.CreateCoupon
{
    public sealed class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<CreateCouponResponse>>
    {
        private readonly IRepository<Coupon> _couponRepository;
        private readonly ICouponQuery _couponQuery;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCouponCommandHandler(IRepository<Coupon> couponRepository, ICouponQuery couponQuery, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _couponQuery = couponQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateCouponResponse>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            var data = await _couponQuery.GetCreateCouponDataAsync(request.Code, request.CourseId, request.UserId, cancellationToken);

            if (!data.CourseExists)
                return Error.NotFound("course.not_found");

            if (!data.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (data.CodeExists)
                return Error.Conflict("coupon.code_already_exists", "Code");

            var couponResult = Coupon.Create(
                 request.Code,
                 request.Value,
                 request.Type,
                 request.ExpirationDate,
                 request.UsageLimit,
                 request.CourseId);

            if (couponResult.IsFailure)
                return couponResult.Error;

            var coupon = couponResult.Value;
            await _couponRepository.AddAsync(coupon, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCouponResponse(
                coupon.Id,
                coupon.Code,
                coupon.Value,
                coupon.Type,
                coupon.ExpirationDate,
                coupon.UsageLimit,
                coupon.CourseId);
        }
    }
}
