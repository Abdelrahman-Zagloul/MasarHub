using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Coupons.Commands.CreateCoupon
{
    public sealed class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
    {
        public CreateCouponCommandValidator()
        {
            RuleFor(x => x.Code)
                .Required("Code")
                .ValidMinLength(3, "Code")
                .ValidMaxLength(50, "Code");

            RuleFor(x => x.Value)
                .ValidGreaterThanZero("Value");

            RuleFor(x => x.Type)
                .ValidEnum("Type");

            RuleFor(x => x.ExpirationDate)
                .Must(date => date > DateTimeOffset.UtcNow)
                .WithErrorCode("Coupon.InvalidExpiration")
                .WithName("ExpirationDate");

            RuleFor(x => x.UsageLimit)
                .ValidGreaterThanZero("UsageLimit");

            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");
        }
    }
}
