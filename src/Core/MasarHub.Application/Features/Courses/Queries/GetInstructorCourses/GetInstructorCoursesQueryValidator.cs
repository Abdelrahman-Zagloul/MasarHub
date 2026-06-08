using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Courses.Queries.GetInstructorCourses
{
    public sealed class GetInstructorCoursesQueryValidator : PaginationValidator<GetInstructorCoursesQuery>
    {
        public GetInstructorCoursesQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .ValidNullableGuid("CategoryId");

            RuleFor(x => x.Language)
                .ValidEnum("Language");

            RuleFor(x => x.Level)
                .ValidEnum("Level");

            RuleFor(x => x.Status)
                .ValidEnum("Status");

            RuleFor(x => x.MinPrice)
                .ValidPrice("MinPrice", 0);

            RuleFor(x => x.MaxPrice)
                .ValidPrice("MaxPrice", 0);

            RuleFor(x => x)
                .Must(x => x.MaxPrice >= x.MinPrice)
                .WithErrorCode("validation.min_price_cannot_exceed_max_price")
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
        }
    }
}
