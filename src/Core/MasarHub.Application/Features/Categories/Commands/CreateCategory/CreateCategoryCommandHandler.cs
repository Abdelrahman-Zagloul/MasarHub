using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Common.Utilities;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Categories;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandHandler
        : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryQuery _categoryQuery;
        private readonly IRepository<Category> _categoryRepository;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryQuery categoryQuery, IRepository<Category> categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryQuery = categoryQuery;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var slug = SlugGenerator.GenerateSlug(request.Name);

            var (displayOrder, slugExist) = await _categoryQuery.GetCreationDataAsync(slug, request.ParentCategoryId, cancellationToken);
            if (slugExist)
                return Error.Conflict("category.slug_already_exists", "Name");

            DomainResult<Category> categoryResult;
            if (request.ParentCategoryId.HasValue)
            {
                var parentCategory = await _categoryQuery.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken);
                if (parentCategory is null)
                    return Error.NotFound("category.not_found");
                categoryResult = Category.CreateSubCategory(request.Name, slug, displayOrder, parentCategory);
            }
            else
                categoryResult = Category.CreateRoot(request.Name, slug, displayOrder);

            if (categoryResult.IsFailure)
                return categoryResult.Error;

            var category = categoryResult.Value;
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCategoryResponse(
                category.Id,
                category.Name,
                category.Slug,
                category.Level,
                category.DisplayOrder,
                category.ParentCategoryId);
        }
    }
}