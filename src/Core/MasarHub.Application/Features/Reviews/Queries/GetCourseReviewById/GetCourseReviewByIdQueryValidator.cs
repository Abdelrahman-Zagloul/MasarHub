using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Reviews.Queries.GetCourseReviewById
{
    public sealed class GetCourseReviewByIdQueryValidator : AbstractValidator<GetCourseReviewByIdQuery>
    {
        public GetCourseReviewByIdQueryValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid("CourseId");

            RuleFor(x => x.ReviewId)
                .ValidGuid("ReviewId");
        }
    }
}
