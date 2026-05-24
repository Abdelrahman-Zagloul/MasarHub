using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Categories.Queries.GetCategoryById
{
    public sealed class GetCategoryByIdQueryHandler
        : IRequestHandler<GetCategoryByIdQuery, Result<CategoryWithChildrenResponse>>
    {
        private readonly ICategoryQuery _categoryQuery;

        public GetCategoryByIdQueryHandler(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }

        public async Task<Result<CategoryWithChildrenResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryQuery.GetWithChildrenByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return Error.NotFound("category.not_found");

            return category;
        }
    }
}
