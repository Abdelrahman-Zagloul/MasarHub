using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.ReorderCategories
{
    public sealed class ReorderCategoriesCommandHandler : IRequestHandler<ReorderCategoriesCommand, Result>
    {
        private readonly ICategoryQuery _categoryQuery;
        public ReorderCategoriesCommandHandler(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }

        public async Task<Result> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
        {
            var existingCategoryIds = await _categoryQuery.GetCategoryIdsByParentIdAsync(request.ParentCategoryId, cancellationToken);

            if (existingCategoryIds.Count != request.OrderedCategoryIds.Count)
                return Error.BadRequest("category.reorder_items_mismatch");

            var orderedCategoryIdsSet = request.OrderedCategoryIds.ToHashSet();

            if (!existingCategoryIds.All(id => orderedCategoryIdsSet.Contains(id)))
                return Error.BadRequest("category.reorder_category_not_found");

            bool isSuccess = await _categoryQuery.BulkUpdateDisplayOrderAsync(request.ParentCategoryId, request.OrderedCategoryIds, cancellationToken);

            if (!isSuccess)
                return Error.Failure("category.reorder_failed");

            return Result.Success();
        }
    }
}
