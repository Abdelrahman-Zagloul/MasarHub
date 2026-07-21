using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Reviews.Commands.DeleteCourseReview
{
    public sealed class DeleteCourseReviewCommandValidator : AbstractValidator<DeleteCourseReviewCommand>
    {
        public DeleteCourseReviewCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.ReviewId)
                .ValidGuid("ReviewId");
        }
    }
}
