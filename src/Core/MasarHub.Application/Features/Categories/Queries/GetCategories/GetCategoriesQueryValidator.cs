using FluentValidation;
using MasarHub.Application.Common.Pagination;

namespace MasarHub.Application.Features.Categories.Queries.GetCategories
{
    public sealed class GetCategoriesQueryValidator : PaginationValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
        {
            RuleFor(x => x.Level)
                .InclusiveBetween(1, 3)
                .When(x => x.Level.HasValue)
                .WithMessage("category.invalid_level");
        }
    }
}
