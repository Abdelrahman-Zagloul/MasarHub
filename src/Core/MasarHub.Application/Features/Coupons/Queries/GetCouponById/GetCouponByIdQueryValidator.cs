using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Coupons.Queries.GetCouponById
{
    public sealed class GetCouponByIdQueryValidator : AbstractValidator<GetCouponByIdQuery>
    {
        public GetCouponByIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.CouponId)
                .ValidGuid("CouponId");
        }
    }
}
