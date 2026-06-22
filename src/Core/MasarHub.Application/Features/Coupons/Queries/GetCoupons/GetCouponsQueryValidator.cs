using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Coupons.Queries.GetCoupons
{
    public sealed class GetCouponsQueryValidator : AbstractValidator<GetCouponsQuery>
    {
        public GetCouponsQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Status)
                .ValidEnum("Status");
        }
    }
}
