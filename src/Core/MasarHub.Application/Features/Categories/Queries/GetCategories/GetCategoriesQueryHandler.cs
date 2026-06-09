using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;

namespace MasarHub.Application.Features.Categories.Queries.GetCategories
{
    public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<PaginatedResult<CategoryResponse>>>
    {
        private readonly ICategoryQuery _categoryQuery;

        public GetCategoriesQueryHandler(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }

        public async Task<Result<PaginatedResult<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _categoryQuery.GetAllAsync(request, cancellationToken);

            return PaginatedResult<CategoryResponse>.Create(pagedResult.Items, pagedResult.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
