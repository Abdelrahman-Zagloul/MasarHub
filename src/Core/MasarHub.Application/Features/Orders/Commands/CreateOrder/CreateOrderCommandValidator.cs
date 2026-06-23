using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleForEach(x => x.Coupons)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.CouponCode)
                        .Required("CouponCode");

                    c.RuleFor(x => x.CourseId)
                        .ValidGuid("CourseId");
                });

            RuleFor(x => x.Coupons)
                .Cascade(CascadeMode.Stop)
                .RequiredNonEmptyCollection("Coupons")
                .Must(coupons => coupons != null && coupons.Select(c => c.CourseId).Distinct().Count() == coupons.Count)
                .WithErrorCode("validation.duplicate_course_coupon")
                .When(x => x.Coupons is not null);
        }
    }
}
