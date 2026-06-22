using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Coupons.Commands.DeleteCoupon
{
    public sealed record DeleteCouponCommand(Guid CourseId, Guid CouponId, Guid UserId) : IRequest<Result>;
}
