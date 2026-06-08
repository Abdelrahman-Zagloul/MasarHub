using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;

namespace MasarHub.Application.Features.Categories.Queries.GetCategories
{
    public sealed record GetCategoriesQuery
    (
        string? CategoryName,
        int? Level,
        int PageNumber = 1,
        int PageSize = 10
    ) : IPaginatedQuery,
        IRequest<Result<PaginatedResult<CategoryResponse>>>;
}
