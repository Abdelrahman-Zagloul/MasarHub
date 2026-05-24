using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;

namespace MasarHub.Application.Features.Categories.Queries.GetCategories
{
    public sealed record GetCategoriesQuery
    (
        int PageNumber,
        int PageSize,
        string? CategoryName,
        int? Level
    ) : IPaginatedQuery,
        IRequest<Result<PaginatedResult<CategoryResponse>>>;
}
