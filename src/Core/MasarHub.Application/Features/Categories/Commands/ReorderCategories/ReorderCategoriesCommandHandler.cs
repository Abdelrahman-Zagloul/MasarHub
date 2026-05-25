using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Categories;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.ReorderCategories
{
    public sealed class ReorderCategoriesCommandHandler : IRequestHandler<ReorderCategoriesCommand, Result>
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReorderCategoriesCommandHandler(IRepository<Category> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ReorderCategoriesCommand request, CancellationToken cancellationToken)
        {
            var categories = request.ParentCategoryId is null
                ? await _categoryRepository.GetAllAsync(x => x.ParentCategoryId == null, cancellationToken)
                : await _categoryRepository.GetAllAsync(x => x.ParentCategoryId == request.ParentCategoryId, cancellationToken);

            if (categories.Count != request.OrderedCategoryIds.Count)
                return Error.BadRequest("category.reorder_items_mismatch");

            var categoryMap = categories.ToDictionary(x => x.Id);
            for (int i = 0; i < request.OrderedCategoryIds.Count; i++)
            {
                var categoryId = request.OrderedCategoryIds[i];
                if (!categoryMap.TryGetValue(categoryId, out var category))
                    return Error.BadRequest("category.reorder_category_not_found");

                var result = category.ChangeDisplayOrder(i + 1);
                if (result.IsFailure)
                    return result.Error;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
