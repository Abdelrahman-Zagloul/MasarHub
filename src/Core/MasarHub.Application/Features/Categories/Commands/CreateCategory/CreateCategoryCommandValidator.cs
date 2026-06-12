using FluentValidation;
using MasarHub.Application.Common.Extensions;

namespace MasarHub.Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .Required("Name")
                .MinLengthValidation(3, "Name")
                .MaxLengthValidation(200, "Name");
        }
    }
}
