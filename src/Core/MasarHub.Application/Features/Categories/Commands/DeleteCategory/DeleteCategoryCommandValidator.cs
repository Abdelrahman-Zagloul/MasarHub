using FluentValidation;

namespace MasarHub.Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("category.not_found");
        }
    }
}
