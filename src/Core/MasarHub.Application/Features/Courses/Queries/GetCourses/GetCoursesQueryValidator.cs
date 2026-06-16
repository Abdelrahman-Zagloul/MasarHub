using FluentValidation;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Courses.Queries.GetCourses
{
    public sealed class GetCoursesQueryValidator : PaginationValidator<GetCoursesQuery>
    {
        public GetCoursesQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .ValidGuid("CategoryId");

            RuleFor(x => x.InstructorId)
                .ValidGuid("InstructorId");

            RuleFor(x => x.Language)
                .ValidEnum("Language");

            RuleFor(x => x.Level)
                .ValidEnum("Level");

            RuleFor(x => x.MinPrice)
                .ValidGreaterThanZero("MinPrice");

            RuleFor(x => x.MaxPrice)
                .ValidGreaterThanZero("MaxPrice");

            RuleFor(x => x)
                .Must(x => x.MaxPrice >= x.MinPrice)
                .WithErrorCode("validation.min_price_cannot_exceed_max_price")
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);
        }
    }
}
