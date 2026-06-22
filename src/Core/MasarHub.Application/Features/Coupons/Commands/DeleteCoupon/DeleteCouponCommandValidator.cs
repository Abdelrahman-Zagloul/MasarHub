using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Coupons.Commands.DeleteCoupon
{
    public sealed class DeleteCouponCommandValidator : AbstractValidator<DeleteCouponCommand>
    {
        public DeleteCouponCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.CouponId)
                .ValidGuid("CouponId");
        }
    }
}
