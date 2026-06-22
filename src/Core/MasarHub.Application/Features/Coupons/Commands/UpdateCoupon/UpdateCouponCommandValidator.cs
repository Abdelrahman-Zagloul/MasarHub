using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Coupons.Commands.UpdateCoupon
{
    public sealed class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
    {
        public UpdateCouponCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.CouponId)
                .ValidGuid("CouponId");

            RuleFor(x => x.Value)
                .ValidGreaterThanZero("Value");

            RuleFor(x => x.UsageLimit)
                .ValidGreaterThanZero("UsageLimit");

            RuleFor(x => x)
                .Must(HaveAtLeastOneUpdate)
                .WithErrorCode("validation.at_least_one_field_required");
        }

        private static bool HaveAtLeastOneUpdate(UpdateCouponCommand command)
        {
            return command.Value.HasValue
                || command.ExpirationDate.HasValue
                || command.UsageLimit.HasValue;
        }
    }
}
