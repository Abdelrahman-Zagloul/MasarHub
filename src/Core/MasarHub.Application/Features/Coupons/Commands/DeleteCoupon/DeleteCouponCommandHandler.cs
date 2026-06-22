using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Payments;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.DeleteCoupon
{
    public sealed class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, Result>
    {
        private readonly IRepository<Coupon> _couponRepository;
        private readonly ICouponQuery _couponQuery;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCouponCommandHandler(IRepository<Coupon> couponRepository, ICouponQuery couponQuery, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _couponQuery = couponQuery;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
        {
            var data = await _couponQuery.GetDeleteCouponDataAsync(request.CouponId, request.UserId, cancellationToken);

            if (data == null)
                return Error.NotFound("coupon.not_found");

            if (!data.IsOwner)
                return Error.Forbidden("course.access_denied");

            if (request.CourseId != data.CourseId)
                return Error.Conflict("coupon.course_mismatch");

            var coupon = await _couponRepository.GetByIdAsync(request.CouponId, cancellationToken);
            if (coupon == null)
                return Error.NotFound("coupon.not_found");

            _couponRepository.Remove(coupon);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
