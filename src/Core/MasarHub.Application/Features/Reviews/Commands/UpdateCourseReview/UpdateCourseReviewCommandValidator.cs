using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Reviews.Commands.UpdateCourseReview
{
    public sealed class UpdateCourseReviewCommandValidator : AbstractValidator<UpdateCourseReviewCommand>
    {
        public UpdateCourseReviewCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.ReviewId)
                .ValidGuid("ReviewId");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithErrorCode("validation.rating_invalid")
                .When(x => x.Rating.HasValue);

            RuleFor(x => x.ReviewContent)
                .ValidMaxLength(2000, "ReviewContent");

            RuleFor(x => x)
                .Must(x => x.Rating.HasValue || x.ReviewContent is not null)
                .WithErrorCode("validation.at_least_one");
        }
    }
}
