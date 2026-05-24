using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Categories;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryQuery _categoryQuery;
        private readonly IRepository<Category> _categoryRepository;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryQuery categoryQuery, IRepository<Category> categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryQuery = categoryQuery;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return Error.NotFound("category.not_found");

            var (hasChildren, hasCourses) = await _categoryQuery.CanDeleteAsync(request.Id, cancellationToken);

            if (hasChildren)
                return Error.Conflict("category.has_children");

            if (hasCourses)
                return Error.Conflict("category.has_courses");

            _categoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
