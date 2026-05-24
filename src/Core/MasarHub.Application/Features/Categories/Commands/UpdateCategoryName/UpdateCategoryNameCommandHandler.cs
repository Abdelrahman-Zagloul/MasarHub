using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Categories;
using MediatR;

namespace MasarHub.Application.Features.Categories.Commands.UpdateCategoryName
{
    public sealed class UpdateCategoryNameCommandHandler : IRequestHandler<UpdateCategoryNameCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;

        public UpdateCategoryNameCommandHandler(IUnitOfWork unitOfWork, IRepository<Category> categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<Result> Handle(UpdateCategoryNameCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return Error.NotFound("category.not_found");

            var result = category.Rename(request.Name);
            if (result.IsFailure)
                return result.Error;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
