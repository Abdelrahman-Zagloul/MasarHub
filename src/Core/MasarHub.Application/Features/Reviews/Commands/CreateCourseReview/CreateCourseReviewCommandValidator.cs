using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Reviews.Commands.CreateCourseReview
{
    public sealed class CreateCourseReviewCommandValidator : AbstractValidator<CreateCourseReviewCommand>
    {
        public CreateCourseReviewCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5)
                .WithErrorCode("validation.rating_invalid");

            RuleFor(x => x.ReviewContent)
                .ValidMaxLength(2000, "ReviewContent");
        }
    }
}
