using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Categories;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ICategoryQuery _categoryQuery;
        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IRepository<Category> categoryRepository, ICategoryQuery categoryQuery)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _categoryQuery = categoryQuery;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return Error.NotFound("category.not_found");

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var result = category.Rename(request.Name);
                if (result.IsFailure)
                    return result.Error;
            }

            if (request.ParentCategoryId.HasValue)
            {
                var hasChildren = await _categoryQuery.HasChildrenAsync(category.Id, cancellationToken);
                if (hasChildren)
                    return Error.BadRequest("category.cannot_change_parent_with_children");

                var parentCategory = await _categoryQuery.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory is null)
                    return Error.NotFound("category.parent_not_found");

                var result = category.ChangeParentCategory(parentCategory);
                if (result.IsFailure)
                    return result.Error;
            }

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
